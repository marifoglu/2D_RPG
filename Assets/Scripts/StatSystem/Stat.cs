using System;
using UnityEngine;

[System.Serializable]
public class Stat 
{
    [SerializeField] private float baseValue;

    public float GetValue()
    {
        return baseValue;
    }
}
