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
        public static Action<String>? LOGGER=null;

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
                    entity.stats.Armor = 0;
                    entity.stats.Damage = 1;
                    entity.stats.BonusActions = 0;
                    entity.stats.MinRollActions = 1;
                    entity.stats.MaxRollActions = 4;
                    entity.stats.Ranged = false;
                }
                else if (entity is NPC)
                {
                    entity.stats.MaxHealth = difficulty.EnemyHP;
                    entity.stats.Armor = 0;
                    entity.stats.Damage = 1;
                    entity.stats.BonusActions = difficulty.EnemyActions;
                    entity.stats.MinRollActions = 0;
                    entity.stats.MaxRollActions = 0;
                    entity.stats.Ranged = false;
                }
                entity.Health = entity.stats.MaxHealth;
            }
        }
    }
}
