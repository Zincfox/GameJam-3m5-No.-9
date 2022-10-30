using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameOverScript : MonoBehaviour
    {

        [SerializeField]
        private GameObject targetObject;

        private GameMaster gameMaster;

        private void Awake()
        {
            if(targetObject == null)
            {
                targetObject = gameObject;
            }
            Debug.Log("Loading GameOverScript");
            gameMaster = FindObjectOfType<GameMaster>();
            if (gameMaster == null) throw new System.Exception("GameMaster not found in Scene!");
            gameMaster.GameStateReadyEvent += (_, _) =>
            {
                foreach(var room in gameMaster.GameState.map.Rooms)
                {
                    foreach(var entity in room.GetRoomEntities())
                    {
                        if(entity is MapMangler.Entities.Player player)
                        {
                            player.DeathEvent += Player_DeathEvent;
                        }
                    }
                }
            };
        }

        private void Player_DeathEvent(object sender, MapMangler.Entities.Entity.EntityDeathEventArgs e)
        {
            gameObject.SetActive(true);
            Debug.LogWarning("!!!GAME OVER!!!");
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}