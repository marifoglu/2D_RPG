using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private ElementType currentElement = ElementType.None;

    public void AppliedChilledEffect(float duration, float slowMultiplier)
    {
        Debug.Log($"Chilled effect applied for {duration} seconds with slow multiplier {slowMultiplier}.");
    }

    public bool CanBeApplied(ElementType newElement)
    {
        return currentElement == ElementType.None;
    }
}
