using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;

    [SerializeField] Skill_DataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private string lockedColorHex = "#9F9797";

    public bool isUnlocked = false;
    public bool isLocked = false;
    private Color lastColor;



    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();

        UpdateIconColor(GetColorByHex(lockedColorHex));
        
    }
    private void Unlock()
    {
        isUnlocked = true;
        UpdateIconColor(Color.white);

        // Find player skill manager
        //unlock skill in player skill manager
    }

    private bool CanBeUnlock()
    {
        if(isLocked || isUnlocked) 
            return false;
        
        return true;
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
        if(!isUnlocked)
        UpdateIconColor(Color.white * .9f);

        ui.skillToolTip.ShowToolTip(true, rect, skillData);
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
        if(!isUnlocked)
            UpdateIconColor(lastColor);

        ui.skillToolTip.ShowToolTip(false, rect );
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
        gameObject.name = "UI - Treenode - " + skillData.displayName;

    }
}
 