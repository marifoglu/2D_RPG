using UnityEngine;

public class ItemEffectDataSO : ScriptableObject
{
    [TextArea]
    public string effectDescription;

    public virtual void ExecuteEffect()
    {
        Debug.Log("Base Item Effect Executed");
    }
}
