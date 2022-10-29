using System;
using System.Collections.Generic;
using System.Text;
using MapMangler.Entities;
using MapMangler.Rooms;
using MapMangler.Rooms.Visibility;
using System.Linq;

namespace MapMangler
{
    public class GameState
    {
        public readonly RoomMap map;
        public readonly VisionTracker playerVisionTracker;
        public readonly VisionTracker.EntityVisionTracker enemyVisionTracker;
        public readonly Difficulty.DifficultyParameters difficulty;

        public GameState(
            VisionTracker playerVisionTracker,
            VisionTracker.EntityVisionTracker enemyVisionTracker,
            Difficulty.DifficultyParameters difficulty,
            RoomMap map)
        {
            this.playerVisionTracker = playerVisionTracker;
            this.enemyVisionTracker = enemyVisionTracker;
            this.difficulty = difficulty;
            this.map = map;
        }
        public GameState(Difficulty.DifficultyParameters difficulty) : this(
            new VisionTracker.EntityVisionTracker(),
            new VisionTracker.EntityVisionTracker(),
            difficulty,
            new RoomMap())
        {

        }

        public void RunSetup()
        {
            foreach (var entity in map.Rooms.SelectMany(r => r.GetRoomEntities()))
            {
                if (entity is Player)
                {
                    entity.stats.MaxHealth = difficulty.PlayerHP;
                }
                else if (entity is NPC)
                {
                    entity.stats.MaxHealth = difficulty.EnemyHP;
                }
                entity.Health = entity.stats.MaxHealth;
            }
        }
    }
}
