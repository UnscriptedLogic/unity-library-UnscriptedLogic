using UnityEngine;
using UnityEngine.UIElements;

namespace UnscriptedLogic.Inventory
{
    public interface IInventoryItem
    {
        string Name { get; }
        string Description { get; }
        int MaxStack { get; }

        void OnAttached<TContext>(TContext context);
        void OnUpdate<TContext>(TContext context);
        void OnRemoved<TContext>(TContext context);
    }

    public class InventorySlot<ItemType> where ItemType : IInventoryItem
    {
        public ItemType item;
        public int amount;
        public bool hasSpace => amount < item.MaxStack;

        public InventorySlot(ItemType item)
        {
            this.item = item;
            amount = 0;
        }

        public void ClearSlot<TContext>(TContext context)
        {
            for (int i = 0; i < amount; i++)
            {
                item.OnRemoved(context);
            }

            amount = 0;
        }
    }

    public class SimpleInventoryHandler<ItemType, TContext> where ItemType : IInventoryItem
    {
        private bool infinteSlots;
        private int maxSlots = 3;
        private List<InventorySlot<ItemType>> inventorySlots;
        private ContextType contextInstance;

        public bool showLogs;

        public bool InfiniteSlots => infinteSlots;
        public int MaxSlots => maxSlots;
        public List<InventorySlot<ItemType>> Slots => inventorySlots;
        public Action<ItemType, int>? OnItemAdded;
        public Action<ItemType, int>? OnItemRemoved;
        public Action<int>? OnSlotsModified;

        public SimpleInventoryHandler(TContext contextType, int maxSlots = -1)
        {
            inventorySlots = new List<InventorySlot<ItemType>>();
            infinteSlots = maxSlots == -1;
            this.maxSlots = maxSlots;
        }

        public void AddItem(ItemType item, int amount = 1)
        {
            if (!infinteSlots)
            {
                if (inventorySlots.Count >= maxSlots)
                {
                    LogMessage("Inventory is full");
                    return;
                }
            }

            InventorySlot<ItemType>? slot = inventorySlots.Find(x => x.item.GetType() == item.GetType());
            if (slot == null)
            {
                slot = new InventorySlot<ItemType>(item);
                inventorySlots.Add(slot);
            }

            if (slot.hasSpace)
            {
                slot.amount += amount;
                slot.item.OnAttached(contextInstance);
            }

            OnItemAdded?.Invoke(item, amount);
        }

        public void RemoveItem(ItemType item, int amount = 1)
        {
            InventorySlot<ItemType>? slot = inventorySlots.Find(x => x.item.Equals(item));
            if (slot == null)
            {
                LogMessage("Item not found");
                return;
            }

            int amountToDrop = Mathf.Min(slot.amount, amount);
            slot.amount -= amountToDrop;
            for (int i = 0; i < amountToDrop; i++)
            {
                slot.item.OnRemoved(contextInstance);
            }

            if (slot.amount <= 0f)
            {
                inventorySlots.Remove(slot);
            }

            OnItemRemoved?.Invoke(item, amount);
        }

        public void AddSlots(int amount)
        {
            maxSlots += amount;
            OnSlotsModified?.Invoke(maxSlots);
        }

        public void RunItemUpdates()
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].item.OnUpdate(contextInstance);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                inventorySlots[i].ClearSlot(contextInstance);
            }

            inventorySlots.Clear();
        }

        public InventorySlot<ItemType>? GetSlot(int index)
        {
            if (index >= inventorySlots.Count)
            {
                LogMessage("An attempt to access an out of bounds inventory slot index occurred.");
                return default;
            }

            return inventorySlots[index];
        }

        private void LogMessage(string message)
        {
            if (!showLogs) return;

            Debug.Log(message);
        }
    }

}
