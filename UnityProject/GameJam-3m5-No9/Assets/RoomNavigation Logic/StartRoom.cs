using UnityEngine;

public class StartRoom : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour[] players;

    [SerializeField]
    private EnemyBehaviour[] enemies;

    public void SetStartLocationsForPlayersAndEnemies()
    {
        var segment = GetComponent<RoomSegment>();
        foreach(var p in players)
        {
            p.Entity.Location = segment.Segment;
        }

        foreach (var e in enemies)
        {
            e.Entity.Location = segment.Segment;
        }
    }
}
