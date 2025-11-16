using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;

    [Header("Unlock Details")]
    public UI_TreeNode[] neededNodes;
    public UI_TreeNode[] conflictNodes;
    public bool isUnlocked;
    public bool isLocked;

    [Header("Skill Details")]
    public Skill_DataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private int skillCost;
    [SerializeField] private string lockedColorHex = "#9F9797";
    private Color lastColor;




    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>();

        UpdateIconColor(GetColorByHex(lockedColorHex));
        
    }
    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        skillTree.RemoveSkillPoints(skillData.cost);
        LockComplateNodes();
        // Find player skill manager
        //unlock skill in player skill manager
    }

    private bool CanBeUnlock()
    {
        if(isLocked || isUnlocked) 
            return false;

        if (skillTree.EnoughSkillPoints(skillData.cost) == false)
            return false;

        foreach (var node in neededNodes)
        {
            if (node.isUnlocked == false)
                return false;
        }

        foreach (var node in conflictNodes)
        {
            if (node.isUnlocked)
                return false;
        }

        return true;
    }

    private void LockComplateNodes()
    {
        foreach (var node in conflictNodes)
        {
            node.isLocked = true;
        }
    }
    private void UpdateIconColor(Color color)
    {
        if(skillIcon == null) 
            return;

        lastColor = skillIcon.color;
        skillIcon.color = color;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, this);

        if(!isUnlocked)
        UpdateIconColor(Color.white * .9f);

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(CanBeUnlock())
            Unlock();
        else
            Debug.Log("Skill cannot be unlocked");
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {

        ui.skillToolTip.ShowToolTip(false, rect);

        if(!isUnlocked)
            UpdateIconColor(lastColor);

    }

    private Color GetColorByHex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        
        return color;
    }

    private void OnValidate()
    {
        if (skillData == null)
            return;

        skillName = skillData.displayName;
        skillIcon.sprite = skillData.icon;
        skillCost = skillData.cost;
        gameObject.name = "UI - Treenode - " + skillData.displayName;

    }
}
 