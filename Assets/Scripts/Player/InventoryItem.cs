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

    [SerializeField]
    [Multiline]
    [Tooltip("The description of the item, visible to the player.")]
    private string m_itemDesc;
    public string GetDescription() { return m_itemDesc; }

    [SerializeField]
    [Tooltip("Can the item be dropped once picked up?")]
    private bool m_droppable = true;
    public bool IsDroppable() { return m_droppable; }

    [SerializeField]
    [Tooltip("The inventory icon for this item.")]
    private Sprite m_sprite;
    public Sprite GetSprite() { return m_sprite; }

    [SerializeField]
    [Tooltip("The inventory model for this item.")]
    private GameObject m_modelPrefab;
    public GameObject GetModel() { return m_modelPrefab; }

    [SerializeField]
    [Tooltip("Does the item stack, e.g. powder.")]
    private bool m_stackable = true;
    public bool DoesStack() { return m_stackable; }

    [SerializeField]
    [Tooltip("Does the item dissapear from inventory when depleted?")]
    private bool m_persistent = false;
    public bool IsPersistent() { return m_persistent; }

    [SerializeField]
    [Tooltip("What does this item turn into when dropped?")]
    private GameObject m_droppedPrefab;
    public GameObject GetDroppedPrefab() { return m_droppedPrefab; }
}
