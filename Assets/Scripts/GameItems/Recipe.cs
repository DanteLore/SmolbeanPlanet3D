using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Smolbean/Recipe", order = 2)]
public class Recipe : ScriptableObject
{
    public Ingredient[] ingredients;
    public float craftingTime = 8f;
    public DropSpec createdItem;
    public int quantity = 1;
}
