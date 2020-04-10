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

        public enum EAlpha
        {
            Clear,
            Faint,
            Half,
            AlmostSolid,
            Solid,
        }

        [Header("Color")]
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

        [Header("Alpha")]
        [SerializeField] private float _faint;
        [SerializeField] private float _almostSolid;

        public Color Get(EColor color, EAlpha alpha = EAlpha.Solid)
        {
            Color toReturn;
            switch (color)
            {
                case EColor.Black:
                    toReturn = _black;
                    break;
                case EColor.DarkGray:
                    toReturn = _darkGray;
                    break;
                case EColor.Gray:
                    toReturn = _gray;
                    break;
                case EColor.LightGray:
                    toReturn = _lightGray;
                    break;
                case EColor.DarkGreen:
                    toReturn = _darkGreen;
                    break;
                case EColor.LightGreen:
                    toReturn = _lightGreen;
                    break;
                case EColor.Red:
                    toReturn = _red;
                    break;
                case EColor.Orange:
                    toReturn = _orange;
                    break;
                case EColor.Yellow:
                    toReturn = _yellow;
                    break;
                case EColor.DarkBrown:
                    toReturn = _darkBrown;
                    break;
                case EColor.Brown:
                    toReturn = _brown;
                    break;
                default:
                    Debug.LogError($"color not implemented: {color}");
                    return Color.clear;
            }

            switch (alpha)
            {
                case EAlpha.Clear:
                    toReturn.a = 0f;
                    break;
                case EAlpha.Faint:
                    toReturn.a = _faint;
                    break;
                case EAlpha.Half:
                    toReturn.a = 0.5f;
                    break;
                case EAlpha.AlmostSolid:
                    toReturn.a = _almostSolid;
                    break;
                case EAlpha.Solid:
                    toReturn.a = 1f;
                    break;
            }

            return toReturn;
        }
    }
}