using UnscriptedLogic.MathUtils;

namespace UnscriptedLogic.Currency
{
    public class CurrencyEventArgs : EventArgs
    {
        public ModifyType modifyType;
        public float changeValue;
        public float currentValue;
    }

    public class CurrencyHandler
    {
        private float starting;
        private float current;
        private float min = 0f;
        private float max = 0f;

        public float Starting => starting;
        public float Current => current;
        public bool IsEmpty => starting == 0f;
        public bool HasNoCap => max == 0f;

        public event EventHandler<CurrencyEventArgs>? OnModified;
        public event EventHandler? OnEmpty;
        public event EventHandler? OnFull;

        public CurrencyHandler(float starting, float min = 0f, float max = 0f, bool setCurrentToStarting = true)
        {
            this.starting = starting;
            this.min = min;
            this.max = max;

            if (setCurrentToStarting)
                current = starting;
        }

        public void Reset() => current = 0f;
        public void ResetToStart() => current = starting;

        public bool HasEnough(float amount) => current >= amount;
        public bool HasEnough(float amount, out float remainder) { remainder = current - amount; return current >= amount; }

        public void SetMinimum(float value) => min = value;
        public void SetMaximum(float value) => max = value;

        public void Modify(ModifyType modifcationType, float amount)
        {
            MathLogic.ModifyValue(modifcationType, ref current, amount);

            if (current < min)
            {
                current = min;
            }

            if (!HasNoCap)
            {
                if (current > max)
                {
                    current = max;
                } 
            }

            OnModified?.Invoke(this, new CurrencyEventArgs()
            {
                modifyType = modifcationType, 
                changeValue = amount, 
                currentValue = current
            });

            if (current <= min)
            {
                OnEmpty?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (current >= max)
            {
                OnFull?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
    }
}
