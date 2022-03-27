using Core;

namespace UI.Actions
{
    class Follow : Action
    {
        public Avatar followAvatar;
        public float followDist;

        public Follow(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("sfollow");
            usageString = parent.RM.GetString("ufollow");

            followAvatar = null;
            followDist = 5;

            client.Objects.OnObjectUpdated += new ObjectManager.ObjectUpdatedCallback(Objects_OnObjectUpdated);
        }

        void Objects_OnObjectUpdated(Simulator simulator, ObjectUpdate update, ulong regionHandle, ushort timeDilation)
        {
            if (followAvatar != null)
            {
                if (LLVector3.Dist(client.Self.SimPosition, followAvatar.Position) > followDist)
                    client.Self.AutoPilotLocal((int)followAvatar.Position.X, 
                        (int)followAvatar.Position.Y, followAvatar.Position.Z);
                else
                    client.Self.AutoPilotCancel();
            }
        }

        public override void acceptInput(string verb, Parser args)
        {
            base.acceptInput(verb, args);

            if (verb == "follow")
            {
                string name = args.objectPhrase;
                Avatar avatar;
                Listeners.Avatars avatars = (Listeners.Avatars)parent.listeners["avatars"];
                if (avatars.tryGetAvatar(name, out avatar))
                {
                    followAvatar = avatar;
                    parent.output("You start to follow " + followAvatar.Name + ".");
                }
                else
                {
                    parent.output("I don't know who " + name + " is.");
                }
            }
            else if (verb == "stop-following") {
                if (followAvatar != null)
                {
                    parent.output("You stop following " + followAvatar.Name + ".");
                    followAvatar = null;
                }
                else
                {
                    parent.output("You aren't following anyone.");
                }
            }

            parent.describeNext = true;
        }
    }
}
