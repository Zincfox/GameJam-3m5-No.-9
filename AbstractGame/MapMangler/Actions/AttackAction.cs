using System;
using System.Collections.Generic;
using System.Text;
using MapMangler.Entities;

namespace MapMangler.Actions
{
    public class AttackAction : GameAction
    {
        public AttackAction(Entity source, Entity target, int damage) : base(source)
        {
            this.target = target;
            this.damage = damage;
        }

        public readonly Entity target;
        public readonly int damage;

        public override int ExpectedCost => 1;

        public override void Perform()
        {
            if (ActionPerformed || Source.Actions < 0)
            {
                return;
            }
            Source.Actions--;
            target.ReceiveBlockableDamage(damage);
            ActionPerformed = true;
        }
    }
}
