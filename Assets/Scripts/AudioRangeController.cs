using UnityEngine;

public class AudioRangeController : MonoBehaviour
{
    private AudioSource audioSource;
    private Transform player;

    [SerializeField] private float minDistanceToHearSound = 5f;
    [SerializeField] private float maxVolume;
    [SerializeField] private bool showGizmos;

    private void Start()
    {
        player = Player.instance.transform;
        audioSource = GetComponent<AudioSource>();

        maxVolume = audioSource.volume;
    }

    private void Update()
    {
        if (player == null || audioSource == null)
            return;
        float distance = Vector2.Distance(player.position, transform.position);
        float t = Mathf.Clamp01(1 - (distance / minDistanceToHearSound));
        audioSource.volume = Mathf.Lerp(0, maxVolume, t * t);
    }
    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, minDistanceToHearSound);
        }
    }
}
