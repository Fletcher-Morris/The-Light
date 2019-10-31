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
    public string GetName() { return m_powderName; }

    [Tooltip("The color associated with this powder, for UI etc.")]
    [SerializeField] private Color m_powderColor;
    public Color GetColor() { return m_powderColor; }

    [Tooltip("The description of this powder, visible to the player.")]
    [Multiline]
    [SerializeField] private string m_description;
    public string GetDescription() { return m_description; }


    [Header("Recipes")]


    [Tooltip("The list or recipes that can create this powder")]
    [SerializeField] private List<PowderRecipe> m_recipes = new List<PowderRecipe>();
    public List<PowderRecipe> GetRecipes() { return m_recipes; }


    [Header("Effects")]


    [SerializeField] private float m_stunPower = 0.0f;
    public float StunPower() { return m_stunPower; }

}

[System.Serializable]
public class PowderRecipe
{
    public List<Powder> Powders = new List<Powder>();
    public int Yield = 1;
}