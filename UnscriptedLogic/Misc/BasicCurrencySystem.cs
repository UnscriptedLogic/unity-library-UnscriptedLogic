using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnscriptedLogic.MathUtils;

namespace UnscriptedLogic.Currency
{
    public class BasicCurrencySystem
    {
        private float starting;
        private float current;

        public float Starting => starting;
        public float Current => current;
        public bool IsEmpty => starting == 0f;

        public Action<ModifyType, float, float>? OnModified;

        public BasicCurrencySystem(float starting, bool setCurrentToStarting = true)
        {
            this.starting = starting;

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

            OnModified?.Invoke(modifcationType, amount, current);
        }
    }
}
