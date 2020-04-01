using System.Collections.Generic;
using UnityEngine;

namespace Tofunaut.Deeepr
{
    [CreateAssetMenu(fileName = "new Color Pallet", menuName = "Deeepr/Color Pallet")]
    public class ColorPallet : ScriptableObject
    {
        public enum EColor
        {
            Black,
            DarkGray,
            Gray,
            LightGray,
            DarkGreen,
            LightGreen,
            Red,
            Orange,
            Yellow,
            DarkBrown,
            Brown,
        }

        [SerializeField] private Color _black;
        [SerializeField] private Color _darkGray;
        [SerializeField] private Color _gray;
        [SerializeField] private Color _lightGray;
        [SerializeField] private Color _darkGreen;
        [SerializeField] private Color _lightGreen;
        [SerializeField] private Color _red;
        [SerializeField] private Color _orange;
        [SerializeField] private Color _yellow;
        [SerializeField] private Color _darkBrown;
        [SerializeField] private Color _brown;

        public Color Get(EColor color)
        {
            switch (color)
            {
                case EColor.Black:
                    return _black;
                case EColor.DarkGray:
                    return _darkGray;
                case EColor.Gray:
                    return _gray;
                case EColor.LightGray:
                    return _lightGray;
                case EColor.DarkGreen:
                    return _darkGreen;
                case EColor.LightGreen:
                    return _lightGreen;
                case EColor.Red:
                    return _red;
                case EColor.Orange:
                    return _orange;
                case EColor.Yellow:
                    return _yellow;
                case EColor.DarkBrown:
                    return _darkBrown;
                case EColor.Brown:
                    return _brown;
                default:
                    Debug.LogError($"color not implemented: {color}");
                    return Color.clear;
            }
        }
    }
}