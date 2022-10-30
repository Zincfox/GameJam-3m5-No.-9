using System;
using System.Collections.Generic;
using System.Text;
using MapMangler.Entities;

namespace MapMangler.Actions
{
    public abstract class GameAction
    {
        private bool actionPerformed = false;
        public bool ActionPerformed
        {
            get => actionPerformed; protected set
            {
                if(ActionPerformed != value)
                {
                    actionPerformed = value;
                    if (value)
                        ActionPerformedEvent?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public abstract int ExpectedCost { get; }

        public Entity Source { get; }

        public GameAction(Entity source)
        {
            Source = source;
        }

        public abstract void Perform();

        public event EventHandler? ActionPerformedEvent;
    }
}
