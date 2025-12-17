using UnityEngine;

public class ItemEffectDataSO : ScriptableObject
{
    [TextArea]
    public string effectDescription;
    protected Player player;

    public virtual bool CanBeUsed(Player player)
    {
        return true;
    }
    public virtual void ExecuteEffect()
    {
        Debug.Log("Base Item Effect Executed");
    }

    public virtual void Subscribe(Player player)
    {
        this.player = player;
    }
    public virtual void Unsubscribe()
    {
    }
}
