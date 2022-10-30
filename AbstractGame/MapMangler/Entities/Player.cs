using System;
using System.Collections.Generic;
using System.Text;
using MapMangler.Difficulty;
using MapMangler.Rooms;

namespace MapMangler.Entities
{
    public class Player : Entity
    {
        public bool IsActive
        {
            get => Actions > 0; set
            {
                if (value && !IsActive)
                {
                    StartTurn();
                }
                else if ((!value) && IsActive)
                {
                    EndTurn();
                }
            }
        }


        public event EventHandler<EntityValueChangeEventArgs<bool>>? ActiveStatusChangedEvent;

        public Player(int entityID) : base(entityID)
        {
            ActionsChangeEvent += Player_ActionsChangeEvent;
        }

        public void StartTurn()
        {
            StartTurn(GlobalRandom.random.Next(stats.MinRollActions, stats.MaxRollActions + 1));
        }

        public void StartTurn(int rolledActions)
        {
            Actions = Math.Max(0, stats.BonusActions + rolledActions);
        }

        public void EndTurn()
        {
            Actions = 0;
        }

        private void Player_ActionsChangeEvent(object sender, EntityValueChangeEventArgs<int> e)
        {
            var wasActive = e.from > 0;
            var isActive = e.to <= 0;
            if (wasActive != isActive)
            {
                ActiveStatusChangedEvent?.Invoke(this, new EntityValueChangeEventArgs<bool>(this, wasActive, isActive));
            }
        }
    }
}
