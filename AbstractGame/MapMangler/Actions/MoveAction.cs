using System;
using System.Collections.Generic;
using System.Text;
using MapMangler.Entities;
using MapMangler.Rooms;
using System.Linq;

namespace MapMangler.Actions
{
    public class MoveAction : GameAction
    {
        public readonly RoomSegmentPath path;

        public MoveAction(Entity source, RoomSegmentPath path) : base(source)
        {
            this.path = path;
        }

        public event EventHandler? ActionStepPerformedEvent;

        public Func<bool> GetStepper()
        {
            IEnumerator<RoomSegment> enumerator = path.roomSegments.Skip(1).GetEnumerator();
            bool stepper()
            {
                if (ActionPerformed) return false;
                if (!enumerator.MoveNext()){
                    ActionPerformed = true;
                    return false;
                };
                if (Source.Actions <= 0) {
                    ActionPerformed = true;
                    return false;
                }
                Source.Actions--;
                Source.Location = enumerator.Current;
                ActionStepPerformedEvent?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return stepper;
        }

        public override int ExpectedCost => path.Cost;

        public override void Perform()
        {
            Func<bool> stepper = GetStepper();
            while (stepper()) { }
        }
    }
}
