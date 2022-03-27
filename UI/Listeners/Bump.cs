using System;
using Core;

namespace UI.Listeners
{
    class Bump : Listener
    {
        public Bump(TextForm parent)
            : base(parent)
        {
            client.Self.OnMeanCollision += new AgentManager.MeanCollisionCallback(Self_OnMeanCollision);
        }

        void Self_OnMeanCollision(MeanCollisionType type, LLUUID perp, LLUUID victim, float magnitude, DateTime time)
        {
            Avatar perpAv, victimAv;
            Listeners.Avatars avatars = (Listeners.Avatars)parent.listeners["avatars"];
            if (avatars.tryGetAvatarById(perp, out perpAv) && avatars.tryGetAvatarById(victim, out victimAv))
            {
                if (victimAv.Name == client.Self.Name)
                    parent.output(perpAv.Name + " bumped into you.");
                else if (perpAv.Name == client.Self.Name)
                    parent.output("You bumped into " + victimAv.Name + ".");
            }
        }
    }
}
