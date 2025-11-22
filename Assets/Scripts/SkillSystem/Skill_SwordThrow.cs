using UnityEngine;

public class Skill_SwordThrow : Skill_Base
{
    [Header("Trajectory prediction")]
    [SerializeField] private GameObject predictionDot;
    [SerializeField] private int numberOfDots = 20;
    [SerializeField] private float spaceBetweenDots = .50f;
    [Space]
    [Range(0f, 10f)]
    [SerializeField] private float throwPower = 6.0f;
    [SerializeField] private float swordGravity = 3.5f;

    private Transform[] dots;
    private Vector2 throwDirection;

    protected override void Awake()
    {
        base.Awake();
        dots = GenerateDots();
    }

    public void ThrowSword()
    {
        Debug.Log("Sword Thrown!");
    }

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
