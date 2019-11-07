using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Powder", menuName = "Powder", order = 1)]
[System.Serializable]
public class Powder : ScriptableObject
{
    [Header("Properties")]


    [Tooltip("The name of the powder, visible to the player.")]
    [SerializeField] private string m_powderName;
    public string PowderName { get => m_powderName; }

    [Tooltip("The color associated with this powder, for UI etc.")]
    [SerializeField] private Color m_powderColor;
    public Color PowderColor { get => m_powderColor; }

    [Tooltip("The description of this powder, visible to the player.")]
    [Multiline]
    [SerializeField] private string m_description;
    public string Description { get => m_description; }


    [Header("Recipes")]


    [Tooltip("The list or recipes that can create this powder")]
    [SerializeField] private List<PowderRecipe> m_recipes = new List<PowderRecipe>();
    public List<PowderRecipe> GetRecipes() { return m_recipes; }


    [Header("Effects")]

    [Tooltip("The how long the lingering effect of this powder last for in seconds.")]
    [SerializeField] private float m_burnTime = 5.0f;
    public float BurnTime { get => m_burnTime; }

    [Tooltip("The stunning power of this powder, higher values stun tougher monsters.")]
    [SerializeField] private float m_stunPower = 0.0f;
    public float StunPower { get => m_stunPower; set => m_stunPower = value; }

    [Tooltip("The fear intensity of this powder, higher values scare tougher monsers.")]
    [SerializeField] private float m_fearIntensity = 0.0f;
    public float FearIntensity { get => m_fearIntensity; }

}

[System.Serializable]
public class PowderRecipe
{
    public List<Powder> Powders = new List<Powder>();
    public int Yield = 1;
}