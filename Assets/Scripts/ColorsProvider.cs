using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Project/ColorsProvider", fileName = "ColorsProvider")]
public class ColorsProvider: ScriptableObject
{
    public enum ColorEnum
    {
        Red,
        Green,
        Blue,
        Purple,
        Yellow
    }
    
    [Serializable]
    public class ColorPair
    {
        [SerializeField] private ColorEnum colorName;
        [SerializeField] private Material material;

        public ColorEnum ColorName => colorName;
        public Material Material => material;
    }

    [SerializeField] private ColorPair[] registeredColors;

    public ColorPair GetByName(ColorEnum colorName)
    {
        return registeredColors.FirstOrDefault(c => c.ColorName == colorName);
    }

    public ColorPair GetRandom()
    {
        return registeredColors[Random.Range(0, registeredColors.Length)];
    }
}