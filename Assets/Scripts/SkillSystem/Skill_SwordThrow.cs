using UnityEngine;

public class Skill_SwordThrow : Skill_Base
{
    private SkillObject_Sword currentSword;

    [Header("Regular Sword upgrade")]
    [SerializeField] private GameObject swordPrefab;
    [Range(0f, 10f)]
    [SerializeField] private float throwPower = 6.0f;

    [Header("Pierce Sword upgrade")]
    [SerializeField] private GameObject pierceSwordPrefab;
    public int pierceAmount = 2;

    [Header("Spin Sword Upgrade")]
    [SerializeField] private GameObject spinSwordPrefab;
    public int maxDistance = 5;
    public float attackPerSecond = 6;
    public int maxSpinDuration = 3;

    [Header("Trajectory prediction")]
    [SerializeField] private GameObject predictionDot;
    [SerializeField] private int numberOfDots = 20;
    [SerializeField] private float spaceBetweenDots = .05f;
    private float swordGravity;
    private Transform[] dots;
    private Vector2 throwDirection;


    protected override void Awake()
    {
        base.Awake();
        swordGravity = swordPrefab.GetComponent<Rigidbody2D>().gravityScale;
        dots = GenerateDots();
    }
    public override bool CanUseSkill()
    {
        if (currentSword != null)
        {
            currentSword.GetSwordBackToPlayer();
            return false;
        }
        return base.CanUseSkill();
    }
    public void ThrowSword()
    {
        GameObject swordPrefab = GetSwordPrefab();

        GameObject newSword = Instantiate(swordPrefab, dots[1].position, Quaternion.identity);

        currentSword = newSword.GetComponent<SkillObject_Sword>();
        currentSword.SetupSword(this, GetThrowPower());
    }

    private GameObject GetSwordPrefab()
    {
        if (Unlocked(SkillUpgradeType.SwordThrow))
            return swordPrefab;
        
        if(Unlocked(SkillUpgradeType.SwordThrow_Pierce))
            return pierceSwordPrefab;

        if(Unlocked(SkillUpgradeType.SwordThrow_Spin))
            return spinSwordPrefab;

        Debug.Log("No valid sword upgrade selected, defaulting to regular sword.");

        return null;
    }


    private Vector2 GetThrowPower() => throwDirection * (throwPower * 10);

    private Vector2 GetTrajectoryPoint(Vector2 direction, float time)
    {
        float scaledThrowPower = throwPower * 10;

        // Gives the initial velocity - starting speed and driection of the throw
        Vector2 initialVelocity = direction * scaledThrowPower;

        // Calculates the effect of gravity over time. The longer the time, the more gravity affects the trajectory
        Vector2 gravityEffect = 0.5f * Physics2D.gravity * swordGravity * (time * time);

        // Final position of the point at the given time
        Vector2 predictionPoint = (initialVelocity * time) + gravityEffect;

        // Adjusts the prediction point relative to the player's position
        Vector2 playerPosition = transform.root.position;

        return playerPosition + predictionPoint;
    }

    public void predictTrajectory(Vector2 direction)
    {
        for(int i = 0; i < dots.Length; i++)    
        {
            dots[i].position = GetTrajectoryPoint(direction, i * spaceBetweenDots);
        }
    }

    public void ConfirmTrajectory(Vector2 direction) => throwDirection = direction;    

    public void EnableDots(bool enable)
    {
        foreach (Transform dot in dots) 
        {
            dot.gameObject.SetActive(enable);
        }
    }

    private Transform[] GenerateDots()
    {
        Transform[] newDots = new Transform[numberOfDots];
        for(int i = 0; i < numberOfDots; i++)
        {
            newDots[i] = Instantiate(predictionDot, transform.position, Quaternion.identity, transform).transform;
            newDots[i].gameObject.SetActive(false);
        }
        return newDots;
    } 

}
