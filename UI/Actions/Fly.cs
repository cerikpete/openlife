﻿namespace UI.Actions
{
    class Fly : Action
    {
        public Fly(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("sfly");
            usageString = parent.RM.GetString("ufly");
        }

        public override void acceptInput(string verb, Parser args)
        {
            base.acceptInput(verb, args);

            parent.output("You are flying.");

            if (args.str == "up")
            {
                client.Self.Movement.UpPos = true;
                client.Self.Movement.SendUpdate(true);
            }
            else if (args.str == "down")
            {
                client.Self.Movement.UpNeg = true;
                client.Self.Movement.SendUpdate(true);
            }
            else
            {
                client.Self.Fly(true);
            }

            parent.describeNext = true;
        }
    }
}

