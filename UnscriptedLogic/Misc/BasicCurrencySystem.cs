using UnscriptedLogic.MathUtils;

namespace UnscriptedLogic.Currency
{
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

        public Action<ModifyType, float, float>? OnModified;
        public Action? OnEmpty;
        public Action? OnFull;

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

        public void Modify(ModifyType modifcationType, float amount)
        {
            RandomLogic.ModifyValue(modifcationType, ref current, amount);

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

            OnModified?.Invoke(modifcationType, amount, current);

            if (current == min)
            {
                OnEmpty?.Invoke();
                return;
            }

            if (current == max)
            {
                OnFull?.Invoke();
                return;
            }
        }
    }
}
