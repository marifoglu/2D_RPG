using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private UI_SkillTree skillTree;
    private UI_TreeConnectHandler connectionHandler;

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
        connectionHandler = GetComponent<UI_TreeConnectHandler>();

        UpdateIconColor(GetColorByHex(lockedColorHex));
    }
     
    public void Refund()
    {
        isUnlocked = false;
        isLocked = false;
        UpdateIconColor(GetColorByHex(lockedColorHex));

        skillTree.AddSkillPoints(skillData.cost);
        connectionHandler.UnlockConnectionImage(false);

        //Skill manager and reset skill
    }
    
    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        LockComplateNodes();

        skillTree.RemoveSkillPoints(skillData.cost);
        connectionHandler.UnlockConnectionImage(true);

        skillTree.skillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData.upgradeData);
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
        if (!isUnlocked && !isLocked)   //not an OR operator
            ToggleNodeHighlight(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(CanBeUnlock())
            Unlock();
        else if(isLocked)
            ui.skillToolTip.LockedSkillEffect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rect);
        if (!isUnlocked && !isLocked) // not an OR operator
            ToggleNodeHighlight(false);

    }

    private void ToggleNodeHighlight(bool highlight)
    {
        Color highlightColor = Color.white * 0.9f; highlightColor.a = 1;
        Color colorToApply = highlight ? highlightColor : lastColor;

        UpdateIconColor(colorToApply);
    }
    private Color GetColorByHex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        
        return color;
    }

    private void OnDisable()
    {
        if (!isUnlocked || isLocked)
            //isLocked not actually needed since it shouldn't highlight to begin with      
            UpdateIconColor(GetColorByHex(lockedColorHex));

        else
            UpdateIconColor(Color.white);
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

 