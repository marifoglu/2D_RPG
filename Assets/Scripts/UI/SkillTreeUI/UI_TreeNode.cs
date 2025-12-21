//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using System.Collections.Generic;

//public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
//{
//    private UI ui;
//    private RectTransform rect;
//    private UI_SkillTree skillTree;
//    private UI_TreeConnectHandler connectionHandler;

//    [Header("Unlock Details")]
//    public UI_TreeNode[] neededNodes;
//    public UI_TreeNode[] conflictNodes;
//    public bool isUnlocked;
//    public bool isLocked;

//    [Header("Skill Details")]
//    public Skill_DataSO skillData;
//    [SerializeField] private string skillName;
//    [SerializeField] private Image skillIcon;
//    [SerializeField] private int skillCost;
//    [SerializeField] private string lockedColorHex = "#9F9797";
//    private Color lastColor;



//    private void Start()
//    {
//        if(isUnlocked == false)
//            UpdateIconColor(GetColorByHex(lockedColorHex));

//        UnlockDefaultSkill();
//    }

//    public void UnlockDefaultSkill()
//    {
//        GetNeededComponents();

//        if (skillData.unlockedByDefault)
//            Unlock();
//    }

//    private void GetNeededComponents()
//    {
//        ui = GetComponentInParent<UI>();
//        rect = GetComponent<RectTransform>();
//        skillTree = GetComponentInParent<UI_SkillTree>(true);
//        connectionHandler = GetComponent<UI_TreeConnectHandler>();
//    }

//    private void OnValidate()
//    {
//        if (skillData == null)
//            return;

//        skillName = skillData.displayName;

//        if (skillIcon != null)
//            skillIcon.sprite = skillData.icon;

//        skillCost = skillData.cost;
//        gameObject.name = "UI - Treenode - " + skillData.displayName;
//    }

//    public void Refund()
//    {
//        if (isUnlocked == false || skillData.unlockedByDefault)
//            return;

//        isUnlocked = false;
//        isLocked = false;
//        UpdateIconColor(GetColorByHex(lockedColorHex));

//        skillTree.AddSkillPoints(skillData.cost);
//        connectionHandler.UnlockConnectionImage(false);

//        //Skill manager and reset skill
//    }

//    private void Unlock()
//    {
//        if (isUnlocked)
//        {
//            Debug.LogWarning("Trying to unlock an already unlocked skill: " + skillData.displayName);
//            return;
//        }

//        isUnlocked = true;
//        UpdateIconColor(Color.white);
//        LockConflictNodes();

//        skillTree.RemoveSkillPoints(skillData.cost);
//        connectionHandler.UnlockConnectionImage(true);

//        skillTree.skillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData);
//    }

//    public void UnlockWithSaveData()
//    {
//        isUnlocked = true;
//        UpdateIconColor(Color.white);
//        LockConflictNodes();

//        connectionHandler.UnlockConnectionImage(true);
//    }

//    private bool CanBeUnlock()
//    {
//        if (isLocked || isUnlocked)
//            return false;

//        if (skillTree.EnoughSkillPoints(skillData.cost) == false)
//            return false;

//        foreach (var node in neededNodes)
//        {
//            if (node.isUnlocked == false)
//                return false;
//        }

//        foreach (var node in conflictNodes)
//        {
//            if (node.isUnlocked)
//                return false;
//        }

//        return true;
//    }

//    //public void LockChildNodes()
//    //{
//    //    LockChildNodes(new HashSet<UI_TreeNode>());
//    //}

//    //private void LockChildNodes(HashSet<UI_TreeNode> visitedNodes)
//    //{
//    //    // Prevent infinite recursion by checking if this node was already visited
//    //    if (visitedNodes.Contains(this))
//    //        return;

//    //    visitedNodes.Add(this);
//    //    isLocked = true;

//    //    foreach (var child in connectionHandler.GetChildNodes())
//    //    {
//    //        if (child != null)
//    //            child.LockChildNodes(visitedNodes);
//    //    }
//    //}
//    public void LockChildNodes()
//    {
//        isLocked = true;

//        foreach (var node in connectionHandler.GetChildNodes())
//            node.LockChildNodes();
//    }

//    private void LockConflictNodes()
//    {
//        foreach (var node in conflictNodes)
//        {
//            node.isLocked = true;
//            node.LockChildNodes();
//        }
//    }

//    private void UpdateIconColor(Color color)
//    {
//        if (skillIcon == null)
//            return;

//        lastColor = skillIcon.color;
//        skillIcon.color = color;
//    }

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        if (ui == null || ui.skillToolTip == null)
//            return;

//        ui.skillToolTip.ShowToolTip(true, rect, skillData, this);
//        if (!isUnlocked && !isLocked)
//            ToggleNodeHighlight(true);
//    }

//    public void OnPointerDown(PointerEventData eventData)
//    {
//        if (ui == null)
//            return;

//        if (CanBeUnlock())
//        {
//            Unlock();
//        }
//        else if (isLocked && ui.skillToolTip != null)
//        {
//            ui.skillToolTip.LockedSkillEffect();
//        }
//    }

//    public void TryUnlockFromGamepad()
//    {
//        if (CanBeUnlock())
//            Unlock();
//        else if (isLocked && ui != null && ui.skillToolTip != null)
//            ui.skillToolTip.LockedSkillEffect();
//    }

//    public void OnPointerExit(PointerEventData eventData)
//    {
//        if (ui == null || ui.skillToolTip == null)
//            return;

//        ui.skillToolTip.ShowToolTip(false, rect);
//        ui.skillToolTip.StopLockedSkillEffect();

//        if (!isUnlocked && !isLocked)
//            ToggleNodeHighlight(false);
//    }

//    private void ToggleNodeHighlight(bool highlight)
//    {
//        Color highlightColor = Color.white * 0.9f; highlightColor.a = 1;
//        Color colorToApply = highlight ? highlightColor : lastColor;

//        UpdateIconColor(colorToApply);
//    }

//    private Color GetColorByHex(string hex)
//    {
//        ColorUtility.TryParseHtmlString(hex, out Color color);
//        return color;
//    }

//    private void OnDisable()
//    {
//        if (!isUnlocked || isLocked)
//            UpdateIconColor(GetColorByHex(lockedColorHex));
//        else
//            UpdateIconColor(Color.white);
//    }
//}











using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

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



    private void Start()
    {
        if (isUnlocked == false)
            UpdateIconColor(GetColorByHex(lockedColorHex));

        UnlockDefaultSkill();
    }

    public void UnlockDefaultSkill()
    {
        GetNeededComponents();

        if (skillData.unlockedByDefault)
            Unlock();
    }

    private void GetNeededComponents()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<UI_SkillTree>(true);
        connectionHandler = GetComponent<UI_TreeConnectHandler>();
    }

    private void OnValidate()
    {
        if (skillData == null)
            return;

        skillName = skillData.displayName;

        if (skillIcon != null)
            skillIcon.sprite = skillData.icon;

        skillCost = skillData.cost;
        gameObject.name = "UI - Treenode - " + skillData.displayName;
    }

    public void Refund()
    {
        if (isUnlocked == false || skillData.unlockedByDefault)
            return;

        isUnlocked = false;
        isLocked = false;
        UpdateIconColor(GetColorByHex(lockedColorHex));

        skillTree.AddSkillPoints(skillData.cost);
        connectionHandler.UnlockConnectionImage(false);

        //Skill manager and reset skill
    }

    private void Unlock()
    {
        if (isUnlocked)
        {
            Debug.LogWarning("Trying to unlock an already unlocked skill: " + skillData.displayName);
            return;
        }

        isUnlocked = true;
        UpdateIconColor(Color.white);
        LockConflictNodes();

        skillTree.RemoveSkillPoints(skillData.cost);
        connectionHandler.UnlockConnectionImage(true);

        skillTree.skillManager.GetSkillByType(skillData.skillType).SetSkillUpgrade(skillData);
    }

    public void UnlockWithSaveData()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);
        LockConflictNodes();

        // FIX: Check if connectionHandler exists before using it
        if (connectionHandler == null)
            connectionHandler = GetComponent<UI_TreeConnectHandler>();

        if (connectionHandler != null)
            connectionHandler.UnlockConnectionImage(true);
        else
            Debug.LogWarning($"ConnectionHandler is null on {gameObject.name} during UnlockWithSaveData");
    }

    private bool CanBeUnlock()
    {
        if (isLocked || isUnlocked)
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

    //public void LockChildNodes()
    //{
    //    LockChildNodes(new HashSet<UI_TreeNode>());
    //}

    //private void LockChildNodes(HashSet<UI_TreeNode> visitedNodes)
    //{
    //    // Prevent infinite recursion by checking if this node was already visited
    //    if (visitedNodes.Contains(this))
    //        return;

    //    visitedNodes.Add(this);
    //    isLocked = true;

    //    foreach (var child in connectionHandler.GetChildNodes())
    //    {
    //        if (child != null)
    //            child.LockChildNodes(visitedNodes);
    //    }
    //}
    public void LockChildNodes()
    {
        // Create a new HashSet for this operation
        HashSet<UI_TreeNode> visitedNodes = new HashSet<UI_TreeNode>();
        LockChildNodes(visitedNodes);
    }

    private void LockChildNodes(HashSet<UI_TreeNode> visitedNodes)
    {
        // Prevent infinite recursion
        if (visitedNodes.Contains(this))
            return;

        visitedNodes.Add(this);

        // FIXED: Use isLocked instead of Lock()
        isLocked = true;

        if (connectionHandler == null)
            return;

        var childNodes = connectionHandler.GetChildNodes();

        if (childNodes == null)
            return;

        foreach (var child in childNodes)
        {
            if (child != null && !child.isLocked) // FIXED: Check if not already locked
            {
                child.LockChildNodes(visitedNodes);
            }
        }
    }

    private void LockConflictNodes()
    {
        // FIXED: Use conflictNodes from UI_TreeNode, not skillData
        if (conflictNodes == null || conflictNodes.Length == 0)
            return;

        foreach (var node in conflictNodes)
        {
            if (node != null && !node.isLocked) // FIXED: Check if not already locked
            {
                node.isLocked = true; // FIXED: Use isLocked directly
                node.LockChildNodes(); // This calls the public method which creates the HashSet
            }
        }
    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)
            return;

        lastColor = skillIcon.color;
        skillIcon.color = color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ui == null || ui.skillToolTip == null)
            return;

        ui.skillToolTip.ShowToolTip(true, rect, skillData, this);
        if (!isUnlocked && !isLocked)
            ToggleNodeHighlight(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (ui == null)
            return;

        if (CanBeUnlock())
        {
            Unlock();
        }
        else if (isLocked && ui.skillToolTip != null)
        {
            ui.skillToolTip.LockedSkillEffect();
        }
    }

    public void TryUnlockFromGamepad()
    {
        if (CanBeUnlock())
            Unlock();
        else if (isLocked && ui != null && ui.skillToolTip != null)
            ui.skillToolTip.LockedSkillEffect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ui == null || ui.skillToolTip == null)
            return;

        ui.skillToolTip.ShowToolTip(false, rect);
        ui.skillToolTip.StopLockedSkillEffect();

        if (!isUnlocked && !isLocked)
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
            UpdateIconColor(GetColorByHex(lockedColorHex));
        else
            UpdateIconColor(Color.white);
    }
}