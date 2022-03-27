using Core;

namespace UI.Actions
{
    class Sit : Action
    {
        public bool sittingOnGround = false;

        public Sit(TextForm parent)
            : base(parent)
        {
            client.Objects.OnAvatarSitChanged += new ObjectManager.AvatarSitChanged(Objects_OnAvatarSitChanged);

            helpString = parent.RM.GetString("ssit");
            usageString = parent.RM.GetString("usit");
        }

        void Objects_OnAvatarSitChanged(Simulator simulator, Avatar avatar, uint sittingOn, uint oldSeat)
        {
            if (avatar.Name == client.Self.Name)
            {
                if (sittingOn != 0)
                    parent.output("You sat down.");
                else
                    parent.output("You stood up.");
            }
            else
            {
                if (sittingOn != 0)
                    parent.output(avatar.Name + " sat down.");
                else
                    parent.output(avatar.Name + " stood up.");
            }
        }

        public override void acceptInput(string verb, Parser args)
        {
            base.acceptInput(verb, args);

            if (client.Self.SittingOn != 0 || sittingOnGround)
                parent.output("You are already sitting.");
            else
            {
                if (args.prepPhrases["on"].Length > 0)
                {
                    string on = args.prepPhrases["on"];
                    Listeners.Objects objects = (Listeners.Objects)parent.listeners["objects"];
                    Primitive prim;
                    if (objects.tryGetPrim(on, out prim))
                    {
                        parent.output("Trying to sit on " + prim.Properties.Name + ".");
                        client.Self.RequestSit(prim.ID, LLVector3.Zero);
                        client.Self.Sit();
                    }
                    else
                    {
                        parent.output("I don't know what " + on + " is.");
                    }
                }
                else
                {
                    parent.output("You sit on the ground.");
                    client.Self.SitOnGround();
                    sittingOnGround = true;
                }
            }

            parent.describeNext = true;
        }
    }
}
