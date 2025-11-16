using UnityEngine;

public class Skill_Dash : Skill_Base
{

    public void OnStartEffect()
    {
        if(Unlocked(SkillUpgradeType.Dash_CloneStart) || Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
            CreateClone();
        
        if(Unlocked(SkillUpgradeType.Dash_ShardOnStart) || Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
            CreateShard();
    }


    public void OnEndEffect()
    {
        if(Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
            CreateClone();
        if(Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
            CreateShard();  
    }

    private void CreateShard()
    {
        Debug.Log("Create Dash Shard");
    }

    private void CreateClone()
    {
        Debug.Log("Create Dash Clone"); 
    }
}
