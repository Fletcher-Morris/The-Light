using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Controller : MonoBehaviour
{
    //  The singleton reference.
    private static Inventory_Controller m_singleton;
    //  Access to the singleton.
    public static Inventory_Controller Singleton() { return m_singleton; }

    private void Awake()
    {
        m_singleton = this;
        ClearInventoryItems();
    }

    //  The individual stacks of items.
    [SerializeField] private List<ItemStack> m_itemStacks;

    //  The base transform of the inventory UI.
    [SerializeField] private Transform m_uiTransform;

    //  The prefab for inventory item UIs.
    [SerializeField] private GameObject m_itemUiPrefab;

    private bool m_open;


    //  Add a quantity of items to the inventory via a reference.
    public void AddItemToInventory(InventoryItem _item, int _quantity)
    {
        //  Non-stackable items.
        if(_item.DoesStack() == false)
        {
            for(int i = 0; i < _quantity; i++)
            {
                bool added = false;
                foreach(ItemStack stack in m_itemStacks)
                {
                    if(stack.item == _item && stack.quantity <= 0)
                    {
                        stack.quantity = 1;
                        added = true;
                        break;
                    }
                }
                if(added == false) m_itemStacks.Add(new ItemStack(_item));
            }
            Debug.Log("Added " + _quantity + " non-stackable items (" + _item.GetName() + ") to inventory.");
            return;
        }

        //  Stackable items.
        foreach(ItemStack stack in m_itemStacks)
        {
            if(stack.item == _item)
            {
                stack.quantity += _quantity;
                Debug.Log("Added " + _quantity + " stackable items (" + _item.GetName() + ") to inventory.");
                return;
            }
        }
        m_itemStacks.Add(new ItemStack(_item, _quantity));
        Debug.Log("Added " + _quantity + " stackable items (" + _item.GetName() + ") to inventory.");
    }

    //  Add a single item to the inventory by reference.
    public void AddItemToInventory(InventoryItem _item)
    {
        AddItemToInventory(_item, 1);
    }


    //  Remove a quantity of items from the inventory via reference.
    public void RemoveItemFromInventory(InventoryItem _item, int _quantity)
    {
        int remainingToRemove = _quantity;
        foreach (ItemStack stack in m_itemStacks)
        {
            if(remainingToRemove > 0)
            {
                if (stack.item == _item)
                {
                    if(stack.quantity >= remainingToRemove)
                    {
                        stack.quantity -= remainingToRemove;
                        remainingToRemove = 0;
                    }
                    else
                    {
                        remainingToRemove -= stack.quantity;
                        stack.quantity = 0;
                    }
                }
            }
        }

        Debug.Log("Removed " + _quantity + " items (" + _item.GetName() + ") from inventory.");
    }

    //  Remove a single item from the inventory via reference.
    public void RemoveItemFromInventory(InventoryItem _item)
    {
        RemoveItemFromInventory(_item, 1);
    }

    //  Remove all instances of an item from the inventory via reference.
    public void RemoveAllItemFromInventory(InventoryItem _item)
    {
        foreach (ItemStack stack in m_itemStacks)
        {
            if (stack.item == _item)
            {
                stack.quantity = 0;
            }
        }
        Debug.Log("Removed all of item (" + _item.GetName() + ") from inventory.");
    }

    //  Remove all inventory items.
    public void ClearInventoryItems()
    {
        m_itemStacks = new List<ItemStack>();
    }


    //  Check if an item exists in the inventory.
    public bool HasItemInInventory(InventoryItem _item)
    {
        foreach(ItemStack stack in m_itemStacks)
        {
            if (stack.item == _item) return true;
        }
        return false;
    }
    //  Check if an item with a given name exists in the inventory.
    public bool HasItemInInventory(string _itemName)
    {
        foreach (ItemStack stack in m_itemStacks)
        {
            if (stack.item.GetName() == _itemName) return true;
        }
        return false;
    }



    public void OpenInventory()
    {
        m_uiTransform.gameObject.SetActive(true);
        foreach (ItemStack stack in m_itemStacks)
        {
            InventoryItem item = stack.item;
            if(stack.quantity <= 0 && item.IsPersistent() == false)
            {

            }
            else
            {
                GameObject stackUi = GameObject.Instantiate(m_itemUiPrefab, m_uiTransform);
                if(!item.IsDroppable())
                {
                    stackUi.transform.GetChild(0).GetComponent<Image>().color = Color.red;
                }
                stackUi.transform.GetChild(1).GetComponent<Text>().text = item.GetName();
                stackUi.transform.GetChild(2).GetComponent<Image>().sprite = item.GetSprite();
                if (item.DoesStack())
                {
                    stackUi.transform.GetChild(3).GetComponent<Text>().text = stack.quantity.ToString();
                }
            }
        }
        m_open = true;
    }

    public void CloseInventory()
    {
        foreach(Transform child in m_uiTransform)
        {
            Destroy(child.gameObject);
        }
        m_uiTransform.gameObject.SetActive(false);
        m_open = false;
    }

    public void ToggleInventory()
    {
        if (m_open) CloseInventory();
        else OpenInventory();
    }

}

[System.Serializable]
public class ItemStack
{
    public InventoryItem item;
    public int quantity;
    public ItemStack(InventoryItem _item, int _q)
    {
        item = _item;
        quantity = _q;
    }
    public ItemStack(InventoryItem _item)
    {
        item = _item;
        quantity = 1;
    }
}