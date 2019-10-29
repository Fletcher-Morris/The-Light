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

    //  The speed at which the selected item rotates.
    [SerializeField] private float m_selectedItemRotateSpeed = 15.0f;

    private void Awake()
    {
        m_singleton = this;
        ClearInventoryItems();
        SetupRtCamera();
    }

    private void Update()
    {
        if(m_selectedItemSpawnedObject != null)
        {
            m_selectedItemSpawnedObject.transform.Rotate(Vector3.up, m_selectedItemRotateSpeed * Time.deltaTime);
        }
    }

    //  The individual stacks of items.
    [SerializeField] private List<ItemStack> m_itemStacks;

    //  The base transform of the inventory UI.
    [SerializeField] private Transform m_uiTransform;

    //  The transform of the main inventory panel.
    [SerializeField] private Transform m_inventoryPanel;
    //  The transform of the main inventory tab.
    [SerializeField] private Image m_inventoryTab;
    //  The transform of the powders mix panel.
    [SerializeField] private Transform m_powdersMixPanel;
    //  The transform of the powders mix tab.
    [SerializeField] private Image m_powdersMixTab;
    //  The transform of the powders wheel panel.
    [SerializeField] private Transform m_powdersWheelPanel;
    //  The transform of the powders wheel tab.
    [SerializeField] private Image m_powdersWheelTab;

    //  The the transform where inventory item prefabs are spawned.
    [SerializeField] private Transform m_inventoryItems;

    //  The prefab for inventory item UIs.
    [SerializeField] private GameObject m_itemUiPrefab;

    //  The sprite to use when noe is available.
    [SerializeField] private Sprite m_errorSprite;

    //  Is the inventory currently open?
    private bool m_open;

    //  The currently selected item stack
    private ItemStack m_selectedStack;
    //  The individual stacks of items.
    [SerializeField] private RenderTexture m_selectedItemRt;
    //  The camera for the render texture.
    private Camera m_selectedItemCam;
    //  The UI image for the selected item.
    [SerializeField] private RawImage m_selectedItemImage;
    //  The UI text for the selected item name.
    [SerializeField] private Text m_selectedItemName;
    //  The UI text for the selected item description.
    [SerializeField] private Text m_selectedItemDescription;
    //  The spawned gameobject representing the selected item.
    private GameObject m_selectedItemSpawnedObject;


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
        ShowInventoryTab();
        foreach (ItemStack stack in m_itemStacks)
        {
            InventoryItem item = stack.item;
            if(stack.quantity <= 0 && item.IsPersistent() == false)
            {

            }
            else
            {
                GameObject stackUi = GameObject.Instantiate(m_itemUiPrefab, m_inventoryItems);
                if(!item.IsDroppable())
                {
                    stackUi.transform.GetChild(0).GetComponent<Image>().color = Color.red;
                }
                stackUi.transform.GetChild(1).GetComponent<Text>().text = item.GetName();
                Sprite useSprite = item.GetSprite();
                if (useSprite == null) useSprite = m_errorSprite;
                stackUi.transform.GetChild(2).GetComponent<Image>().sprite = useSprite;                
                if (item.DoesStack())
                {
                    stackUi.transform.GetChild(3).GetComponent<Text>().text = stack.quantity.ToString();
                }
                stackUi.GetComponent<Button>().onClick.AddListener(() => SelectItemStack(stack));
            }
        }
        if(m_itemStacks.Count > 0) SelectItemStack(m_itemStacks[0]);
        m_open = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseInventory()
    {
        foreach(Transform child in m_inventoryItems)
        {
            Destroy(child.gameObject);
        }
        m_uiTransform.gameObject.SetActive(false);
        m_open = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ToggleInventory()
    {
        if (m_open) CloseInventory();
        else OpenInventory();
    }



    //  Select a given item stack.
    public void SelectItemStack(ItemStack _stack)
    {
        if (_stack == null) return;

        m_selectedStack = _stack;
        if(m_selectedItemSpawnedObject != null)
        {
            GameObject.Destroy(m_selectedItemSpawnedObject);
        }

        if(_stack.item.GetModel() != null)
        {
            m_selectedItemSpawnedObject = Instantiate(_stack.item.GetModel(), m_selectedItemCam.transform);
            m_selectedItemSpawnedObject.transform.localPosition = new Vector3(0, 0, 2.5f);
            int layer = LayerMask.NameToLayer("SelectedItem");
            foreach(Transform c in m_selectedItemSpawnedObject.transform)
            {
                c.gameObject.layer = layer;
            }
            m_selectedItemSpawnedObject.layer = layer;
        }

        m_selectedItemName.text = _stack.item.GetName();
        m_selectedItemDescription.text = _stack.item.GetDescription();
    }



    //  Set up the render texture and camera for the selected item.
    private void SetupRtCamera()
    {
        if(m_selectedItemCam == null)
        {
            GameObject newCam = new GameObject("INVENTORY_ITEM_CAMERA");
            m_selectedItemCam = newCam.AddComponent<Camera>();
            m_selectedItemCam.transform.position = new Vector3(0, -1000, 0);
            m_selectedItemCam.cullingMask = LayerTools.CreateLayerMask("SelectedItem");
            m_selectedItemCam.targetTexture = m_selectedItemRt;
            m_selectedItemCam.clearFlags = CameraClearFlags.Depth;
        }
    }




    //  Show the main inventory tab.
    public void ShowInventoryTab()
    {
        m_inventoryPanel.gameObject.SetActive(true);
        m_inventoryTab.enabled = true;
        m_powdersMixPanel.gameObject.SetActive(false);
        m_powdersMixTab.enabled = false;
        m_powdersWheelPanel.gameObject.SetActive(false);
        m_powdersWheelTab.enabled = false;
    }
    //  Show the powders mix tab.
    public void ShowPowdersMixTab()
    {
        m_inventoryPanel.gameObject.SetActive(false);
        m_inventoryTab.enabled = false;
        m_powdersMixPanel.gameObject.SetActive(true);
        m_powdersMixTab.enabled = true;
        m_powdersWheelPanel.gameObject.SetActive(false);
        m_powdersWheelTab.enabled = false;
    }
    //  Show the powders wheel tab.
    public void ShowPowdersWheelTab()
    {
        m_inventoryPanel.gameObject.SetActive(false);
        m_inventoryTab.enabled = false;
        m_powdersMixPanel.gameObject.SetActive(false);
        m_powdersMixTab.enabled = false;
        m_powdersWheelPanel.gameObject.SetActive(true);
        m_powdersWheelTab.enabled = true;
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