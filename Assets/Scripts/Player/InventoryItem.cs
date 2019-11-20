using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory Item", order = 1)]
[System.Serializable]
public class InventoryItem : ScriptableObject
{
    [SerializeField]
    [Tooltip("The name of the item visible to the player.")]
    private string m_itemName;
    public string GetName() { return m_itemName; }

    [Tooltip("The description of the item, visible to the player.")]
    [SerializeField] [Multiline] private string m_itemDesc;
    public string GetDescription() { return m_itemDesc; }

    [Tooltip("Can the item be dropped once picked up?")]
    [SerializeField]  private bool m_droppable = true;
    public bool IsDroppable() { return m_droppable; }

    [Tooltip("The inventory icon for this item.")]
    [SerializeField] private Sprite m_sprite;
    public Sprite GetSprite() { return m_sprite; }

    [Tooltip("The inventory model for this item.")]
    [SerializeField] private GameObject m_modelPrefab;
    public GameObject GetModel() { return m_modelPrefab; }

    [Tooltip("Does the item stack, e.g. powder.")]
    [SerializeField]  private bool m_stackable = true;
    public bool DoesStack() { return m_stackable; }

    [Tooltip("Does the item dissapear from inventory when depleted?")]
    [SerializeField]  private bool m_persistent = false;
    public bool IsPersistent() { return m_persistent; }

    [Tooltip("What does this item turn into when dropped?")]
    [SerializeField] private GameObject m_droppedPrefab;
    public GameObject GetDroppedPrefab() { return m_droppedPrefab; }

    [Tooltip("The scale of the item when viewed in the inventory.")]
    [SerializeField] private float m_inventoryItemScale = 1.0f;
    public float InventoryItemScale { get => m_inventoryItemScale; }
}
