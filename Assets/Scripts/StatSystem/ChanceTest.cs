using UnityEngine;

public class ChanceTest : MonoBehaviour
{
    [SerializeField] private float chance = 25f;
    [SerializeField] private float rollResult;
    [SerializeField] private float result;

    [ContextMenu("Try")]
    public void Try()
    {
        rollResult = Random.Range(0f, 100f);

        if(rollResult < chance)
        {
            result = 1f; // Success
            Debug.Log($"Success! Rolled {rollResult} against chance {chance}");
        }
        else
        {
            result = 0f; // Failure
            Debug.Log($"Failure! Rolled {rollResult} against chance {chance}");
        }
    }

}
