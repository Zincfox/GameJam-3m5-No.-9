using System;
using System.Collections.Generic;
using System.Text;

namespace MapMangler.Difficulty
{
    public enum DifficultyLevel
    {
        EASY,
        MEDIUM,
        HARD,
        HELL,
    }

    public class DifficultyParameters
    {
        public readonly DifficultyLevel difficulty;
        public readonly int PlayerHP;
        public readonly int EnemyHP;
        public readonly int EnemyActions;

        public DifficultyParameters(int playerHP, int enemyHP, int enemyActions, DifficultyLevel difficulty)
        {
            PlayerHP = playerHP;
            EnemyHP = enemyHP;
            EnemyActions = enemyActions;
            this.difficulty = difficulty;
        }

        private static readonly Dictionary<int, Dictionary<DifficultyLevel, DifficultyParameters>> templates = new Dictionary<int, Dictionary<DifficultyLevel, DifficultyParameters>>
        {
            [4] = new Dictionary<DifficultyLevel, DifficultyParameters>
            {
                [DifficultyLevel.EASY] = new DifficultyParameters(3, 11, 2, DifficultyLevel.EASY),
                [DifficultyLevel.MEDIUM] = new DifficultyParameters(3, 12, 2, DifficultyLevel.MEDIUM),
                [DifficultyLevel.HARD] = new DifficultyParameters(2, 13, 2, DifficultyLevel.HARD),
                [DifficultyLevel.HELL] = new DifficultyParameters(3, 13, 3, DifficultyLevel.HELL)
            },
            [3] = new Dictionary<DifficultyLevel, DifficultyParameters>
            {
                [DifficultyLevel.EASY] = new DifficultyParameters(3, 8, 2, DifficultyLevel.EASY),
                [DifficultyLevel.MEDIUM] = new DifficultyParameters(3, 9, 2, DifficultyLevel.MEDIUM),
                [DifficultyLevel.HARD] = new DifficultyParameters(2, 10, 2, DifficultyLevel.HARD),
                [DifficultyLevel.HELL] = new DifficultyParameters(3, 10, 3, DifficultyLevel.HELL)
            },
            [2] = new Dictionary<DifficultyLevel, DifficultyParameters>
            {
                [DifficultyLevel.EASY] = new DifficultyParameters(3, 5, 2, DifficultyLevel.EASY),
                [DifficultyLevel.MEDIUM] = new DifficultyParameters(3, 6, 2, DifficultyLevel.MEDIUM),
                [DifficultyLevel.HARD] = new DifficultyParameters(2, 7, 2, DifficultyLevel.HARD),
                [DifficultyLevel.HELL] = new DifficultyParameters(3, 7, 3, DifficultyLevel.HELL)
            }
        };

        public static DifficultyParameters fromLevel(DifficultyLevel level, int playerCount)
        {
            return templates[playerCount][level];
        }
    }
}
