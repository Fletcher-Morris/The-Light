using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Add_To_Inventory : MonoBehaviour
{
    //  What item to add to the inventory.
    [SerializeField] private InventoryItem m_item;

    //  How many items be fore depletion (-1 is infinite).
    [SerializeField] private int m_remaining = 1;

    //  Should the object be destroyed when depleted?
    [SerializeField] private bool m_destroyOnDepletion = true;

    //  Should the object be destroyed when used?
    [SerializeField] private bool m_destroyOnAdd = false;

    //  Should the trigger connectd to this object be disabled when depleted?
    [SerializeField] private bool m_disableTriggerOnDepletion = true;

    //  What to do when added to inventory.
    public UnityEvent OnAddedToInventory;

    //  What to do when depleted.
    public UnityEvent OnDepleted;



    //  Add the item.
    public void AddToInventory()
    {
        AddToInventory(1);
    }

    //  Add a given number of items.
    public void AddToInventory(int _quantity)
    {
        if (m_item == null) return;
        if(m_remaining >= 0)
        {
            int i = 0;
            while(m_remaining > 0 && i < _quantity)
            {
                i++;
                m_remaining--;
            }
            Inventory_Controller.Singleton().AddItemToInventory(m_item, i);
            if (m_remaining == 0)
            {
                OnDepleted.Invoke();
                if(m_disableTriggerOnDepletion) GetComponent<Interact_Trigger>()?.DestroyTrigger();
                if (m_destroyOnDepletion) GameObject.Destroy(gameObject);
            }
        }
        else
        {
            Inventory_Controller.Singleton().AddItemToInventory(m_item, _quantity);
        }
        OnAddedToInventory.Invoke();
        if (m_destroyOnAdd) GameObject.Destroy(gameObject);
    }
}
