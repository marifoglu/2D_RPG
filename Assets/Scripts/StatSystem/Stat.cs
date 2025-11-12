using System;
using UnityEngine;
using System.Collections.Generic;


[Serializable]
public class Stat 
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();

    private bool needToCalculated = true;
    private float finalValue;

    public float GetValue()
    {
        if (needToCalculated)
        {
            finalValue = GetFinalValue();
            needToCalculated = false;
        }

        return finalValue;
    }
    public void AddModifier(float value, string source)
    {
        StatModifier modToAdd = new StatModifier(value, source);
        modifiers.Add(modToAdd);
        needToCalculated = true;
    }

    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(mod => mod.source == source);
        needToCalculated = true;

        //foreach (var mod in modifiers)
        //{
        //    Debug.Log($"Remaining Modifier: {mod.source} with value {mod.value}");
        //}
    }

    public float GetFinalValue()
    {
        finalValue = baseValue;

        foreach (var mod in modifiers)
        {
            finalValue += mod.value;
        }
        return finalValue;
    }
}

[Serializable]
public class StatModifier
{
    public float value;
    public string source;

    public StatModifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }
}
