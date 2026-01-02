using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Dialogue Data/New Line Data", fileName = "Line - ")]
public class DialogueLineSO : ScriptableObject
{
    [Header("Dialogue Line Info")]
    public string dialogueGroupName;
    public DialogueSpeakerSO speaker;

    [Header("Text Options")]
    [TextArea] public string[] textLine;

    [Header("Choice Options")]
    [TextArea] public string playerChoiceAnswer;
    public DialogueLineSO[] choiceLines;

    [Header("Dialogue Options")]
    [TextArea] public string actionLine;
    public DialogueActionType actionType; 

    public string GetFirstLine() => textLine[0];

    public string GetRandomLine()
    {
        return textLine[Random.Range(0, textLine.Length)];
    }
}
