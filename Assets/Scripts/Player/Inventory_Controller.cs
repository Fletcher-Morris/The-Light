using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

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

        while(m_powderQuantities.Count < m_allPowders.Count)
        {
            m_powderQuantities.Add(0);
        }
        while (m_powderQuantities.Count > m_allPowders.Count)
        {
            m_powderQuantities.RemoveAt(m_powderQuantities.Count - 1);
        }
    }

    private void Update()
    {
        if(m_selectedItemSpawnedObject != null)
        {
            m_selectedItemSpawnedObject.transform.Rotate(Vector3.up, m_selectedItemRotateSpeed * Time.deltaTime);
        }
        m_selectedItemCam.enabled = m_inventoryPanel.gameObject.activeInHierarchy;
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


    [Header("Main Inventory")]


    //  The the transform where inventory item prefabs are spawned.
    [SerializeField] private Transform m_inventoryItems;
    //  The prefab for inventory item UIs.
    [SerializeField] private GameObject m_itemUiPrefab;
    //  The sprite to use when noe is available.
    [SerializeField] private Sprite m_errorSprite;
    //  Is the inventory currently open?
    private bool m_open;
    public bool IsOpen() { return m_open; }
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
    //  The button that drops items from the inventory.
    [SerializeField] private Button m_dropItemBtn;
    //  The spawned gameobject representing the selected item.
    private GameObject m_selectedItemSpawnedObject;


    [Header("Powders Inventory")]

    //  All powders used in the game.
    [SerializeField] private List<Powder> m_allPowders = new List<Powder>();
    //  The quantities of powders possessed by the player.
    [SerializeField] private List<int> m_powderQuantities = new List<int>();
    //  The transform of the powders wheel.
    [SerializeField] private Transform m_powdersWheel;
    //  The transform where powder ui items are spawned.
    [SerializeField] private Transform m_powdersItems;
    //  The sprite used for powders.
    [SerializeField] private Sprite m_powderSprite;


    //  Add a quantity of items to the inventory via a reference.
    public void AddItemToInventory(InventoryItem _item, int _quantity)
    {
        if (_quantity <= 0) return;

        if (_item.DoesStack() == false)
        {
            //  Non-stackable items.
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
                if(added == false)
                {
                    m_itemStacks.Add(new ItemStack(_item));
                }
            }
            Debug.Log("Added " + _quantity + " non-stackable items (" + _item.GetName() + ") to inventory.");
        }
        else
        {
            //  Stackable items.
            bool added = false;
            foreach (ItemStack stack in m_itemStacks)
            {
                if (stack.item == _item)
                {
                    stack.quantity += _quantity;
                    Debug.Log("Added " + _quantity + " stackable items (" + _item.GetName() + ") to inventory.");
                    added = true;
                    break;
                }
            }
            if(added == false) m_itemStacks.Add(new ItemStack(_item, _quantity));
            Debug.Log("Added " + _quantity + " stackable items (" + _item.GetName() + ") to inventory.");
        }        
        RefreshMainInventory();


        if(Player_Controller.Singleton().ActivePowderStack.item == null)
        {
            if(_item is Powder)
            {
                Debug.Log("Added Item Is Powder");
                Player_Controller.Singleton().ActivePowderStack = GetPowderItemStack();
            }
            else
            {
                Debug.Log("Added Item Is Not Powder");
            }
        }
        else
        {
            Debug.Log("Active Powder Stack Is Not Null");
        }
    }

    //  Get Powder Item Stack
    ItemStack GetPowderItemStack()
    {
        foreach(ItemStack stack in m_itemStacks)
        {
            if(stack.item is Powder)
            {
                if(stack.quantity > 0)
                {
                    return stack;
                }
            }
        }
        Debug.Log("Couldnt find powder item stack with quantity at least 1!");
        return null;
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

        RefreshMainInventory();
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
        RefreshMainInventory();
    }

    //  Remove all inventory items.
    public void ClearInventoryItems()
    {
        m_itemStacks = new List<ItemStack>();
        SelectItemStack(null);
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

    //  Drop the selected item.
    public void DropSelectedItem()
    {
        TryDropItem(m_selectedStack.item);
    }
    //  Drop a specific item.
    public void TryDropItem(InventoryItem _item)
    {
        if(HasItemInInventory(_item))
        {
            RemoveItemFromInventory(_item);
            SpawnItemAtPlayer(_item);
        }
        RefreshMainInventory();
    }
    //  Drop a specific item.
    public void ForceDropItem(InventoryItem _item)
    {
        RemoveItemFromInventory(_item);
        SpawnItemAtPlayer(_item);
        RefreshMainInventory();
    }
    //  Spawn a specific item at the player's location.
    public void SpawnItemAtPlayer(InventoryItem _item)
    {
        if(_item.GetDroppedPrefab() != null)
        GameObject.Instantiate(_item.GetDroppedPrefab(), transform.position + new Vector3(0,2,0), Quaternion.identity);
    }

    public void OpenInventory()
    {
        m_uiTransform.gameObject.SetActive(true);
        ShowInventoryTab();
        GameTime.Pause();
        m_open = true;
    }

    public void RefreshMainInventory()
    {
        if (!m_inventoryPanel.gameObject.activeInHierarchy) return;
        foreach (Transform child in m_inventoryItems)
        {
            Destroy(child.gameObject);
        }
        foreach (ItemStack stack in m_itemStacks)
        {
            InventoryItem item = stack.item;
            if (stack.quantity <= 0 && item.IsPersistent() == false)
            {

            }
            else
            {
                GameObject stackUi = GameObject.Instantiate(m_itemUiPrefab, m_inventoryItems);
                if (!item.IsDroppable())
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
        SelectAppropriateStack();
    }

    private void SelectAppropriateStack()
    {
        if (m_itemStacks.Count == 0)
        {
            SelectItemStack(null);
            return;
        }
        if(m_selectedStack != null)
        {
            if (m_selectedStack.quantity > 0) return;
        }
        for(int i = m_itemStacks.Count - 1; i >= 0; i--)
        {
            if(m_itemStacks[i].item.IsPersistent() || m_itemStacks[i].quantity > 0)
            {
                SelectItemStack(m_itemStacks[i]);
                return;
            }
        }
        SelectItemStack(null);
    }

    public void CloseInventory()
    {
        foreach(Transform child in m_inventoryItems)
        {
            Destroy(child.gameObject);
        }
        m_uiTransform.gameObject.SetActive(false);
        m_open = false;
        m_selectedStack = null;
        GameObject.Destroy(m_selectedItemSpawnedObject);
        GameTime.UnPause();
    }

    public void ToggleInventory()
    {
        if (m_open) CloseInventory();
        else OpenInventory();
    }



    //  Select a given item stack.
    public void SelectItemStack(ItemStack _stack)
    {
        if (_stack == null)
        {
            if (m_selectedItemSpawnedObject != null)
            {
                GameObject.Destroy(m_selectedItemSpawnedObject);
            }
            m_selectedItemName.text = "";
            m_selectedItemDescription.text = "";
            m_dropItemBtn.gameObject.SetActive(false);
            return;
        }
        else if(_stack == m_selectedStack)
        {

        }
        else
        {
            m_selectedStack = _stack;
            if (m_selectedItemSpawnedObject != null)
            {
                GameObject.Destroy(m_selectedItemSpawnedObject);
            }

            if (_stack.item.GetModel() != null)
            {
                m_selectedItemSpawnedObject = Instantiate(_stack.item.GetModel(), m_selectedItemCam.transform);
                m_selectedItemSpawnedObject.transform.localPosition = new Vector3(0, 0, 2.5f) + _stack.item.InventoryItemOffset;
                float s = _stack.item.InventoryItemScale;
                m_selectedItemSpawnedObject.transform.localScale = new Vector3(s, s, s);
                Rigidbody b = m_selectedItemSpawnedObject.GetComponent<Rigidbody>();
                if (b) b.isKinematic = true;
                LayerTools.ChangeLayerRecursive(m_selectedItemSpawnedObject, "SelectedItem");
                m_selectedItemSpawnedObject.GetComponent<Interact_Trigger>()?.SetInteractable(false);
            }

            m_selectedItemName.text = _stack.item.GetName();
            m_selectedItemDescription.text = _stack.item.GetDescription();
            m_dropItemBtn.gameObject.SetActive((_stack.item.IsDroppable() && _stack.quantity > 0));
        }
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
            PostProcessLayer post = newCam.AddComponent<PostProcessLayer>();
            post.volumeTrigger = newCam.transform;
            post.volumeLayer = LayerMask.NameToLayer("PostProcessing");
            post.antialiasingMode = PostProcessLayer.Antialiasing.None;
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
        RefreshMainInventory();
    }
    ////  Show the powders mix tab.
    //public void ShowPowdersMixTab()
    //{
    //    m_inventoryPanel.gameObject.SetActive(false);
    //    m_inventoryTab.enabled = false;
    //    m_powdersMixPanel.gameObject.SetActive(true);
    //    m_powdersMixTab.enabled = true;
    //    m_powdersWheelPanel.gameObject.SetActive(false);
    //    m_powdersWheelTab.enabled = false;
    //}
    ////  Show the powders wheel tab.
    //public void ShowPowdersWheelTab()
    //{
    //    m_inventoryPanel.gameObject.SetActive(false);
    //    m_inventoryTab.enabled = false;
    //    m_powdersMixPanel.gameObject.SetActive(false);
    //    m_powdersMixTab.enabled = false;
    //    m_powdersWheelPanel.gameObject.SetActive(true);
    //    m_powdersWheelTab.enabled = true;
    //    RefreshPowdersInventory();
    //}



    //public int GetPowderId(Powder _powder)
    //{
    //    int id = -1;
    //    for (int i = 0; i < m_allPowders.Count; i++)
    //    {
    //        if (m_allPowders[i] == _powder)
    //        {
    //            id = i;
    //            break;
    //        }
    //    }
    //    return id;
    //}

    //public void AddPowderToInventory(Powder _powder)
    //{
    //    AddPowderToInventory(_powder, 1);
    //}
    //public void AddPowderToInventory(Powder _powder, int _quantity)
    //{
    //    int id = GetPowderId(_powder);
    //    if (id == -1)
    //    {
    //        id = m_allPowders.Count;
    //        m_allPowders.Add(_powder);
    //        m_powderQuantities.Add(_quantity);
    //    }
    //    RefreshPowdersInventory();
    //}

    //public void RefreshPowdersInventory()
    //{
    //    if (!m_powdersWheelPanel.gameObject.activeInHierarchy) return;
    //    foreach (Transform child in m_powdersItems)
    //    {
    //        Destroy(child.gameObject);
    //    }
    //    foreach (Powder powder in m_allPowders)
    //    {
    //        GameObject pow = new GameObject(powder.GetName());
    //        pow.transform.SetParent(m_powdersItems);
    //        pow.transform.localScale = Vector3.one;
    //        Image img = pow.AddComponent<Image>();
    //        img.color = powder.PowderColor;
    //        img.sprite = m_powderSprite;
    //        Button btn = pow.AddComponent<Button>();
    //        btn.onClick.AddListener(() => SelectInventoryPowder(powder));
    //    }
    //}

    //private Powder m_selectedPowder;
    //private int m_selectedPowderId;

    //public void SelectInventoryPowder(Powder _powder)
    //{
    //    m_selectedPowder = _powder;
    //    m_selectedPowderId = GetPowderId(_powder);

    //}

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