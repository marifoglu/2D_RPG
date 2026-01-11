using UnityEngine;

public class Enemy_ArchElf_Arrow : MonoBehaviour, ICounterable
{
    [SerializeField] private LayerMask whatIsTarget;
    //  layername: EnemyProjectile 

    private Collider2D col;
    private Rigidbody2D rb;
    private Animator anim;
    private Entity_Combat combat;

    public bool CanBeCountered => true;

    public void SetupArrow(float xVelocity, Entity_Combat combat)
    {
        rb= GetComponent<Rigidbody2D>();
        col= GetComponent<Collider2D>();
        anim= GetComponentInChildren<Animator>();

        this.combat = combat;
        
        rb.linearVelocity = new Vector2(xVelocity, 0f);
        if(rb.linearVelocity.x < 0 )
            transform.Rotate(0, 180, 0);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ensure arrow is properly setup
        if (combat == null)
            return;

        // Check if collided with target layer
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0)
        {
            // Damage Target
            combat.PerformAttackOnTarget(collision.transform);
            StuckIntoTarget(collision.transform);
        }
    }

    private void StuckIntoTarget(Transform target)
    {
        rb.linearVelocity = Vector2.zero; 
        rb.bodyType = RigidbodyType2D.Kinematic;
        col.enabled = false;
        anim.enabled = false;

        transform.parent = target;

        Destroy(gameObject, 3f);
    }

    public void HandleCounter()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * -1, 0);
        transform.Rotate(0, 180, 0);

        int enemyLayer = LayerMask.NameToLayer("Enemy");

        whatIsTarget = whatIsTarget | (1 << enemyLayer);
    }
}
