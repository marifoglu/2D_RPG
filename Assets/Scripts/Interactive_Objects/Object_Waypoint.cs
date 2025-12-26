using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_Waypoint : MonoBehaviour
{
    [SerializeField] private string transferToScene;
    [Space]
    [SerializeField] public RespawnType waypointType;
    [SerializeField] private RespawnType contendWaypoint;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool canBeTriggered = true;

    public RespawnType GetWaypointType() => waypointType;

    public Vector3 GetPositionAndSetTriggerFalse()
    {
        canBeTriggered = false;
        return respawnPoint == null ? transform.position : respawnPoint.position;
    }

    private void OnValidate()
    {
        gameObject.name = "Object_Type  - " + waypointType.ToString() + " - " + transferToScene;

        if (waypointType == RespawnType.Enter && contendWaypoint != RespawnType.Exit)
            contendWaypoint = RespawnType.Exit;
        else if (waypointType == RespawnType.Exit && contendWaypoint != RespawnType.Enter)
            contendWaypoint = RespawnType.Enter;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeTriggered == false)
            return;

        GameManager.instance.ChangeScene(transferToScene, contendWaypoint);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTriggered = true;
    }
}
