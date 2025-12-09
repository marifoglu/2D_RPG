using UnityEngine;

public class ItemEffectDataSO : ScriptableObject
{
    [TextArea]
    public string effectDescription;

    public virtual bool CanBeUsed()
    {
        return true;
    }
    public virtual void ExecuteEffect()
    {
        Debug.Log("Base Item Effect Executed");
    }
}
