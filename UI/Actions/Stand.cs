namespace UI.Actions
{
    class Stand : Action
    {
        public Stand(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("sstand");
            usageString = parent.RM.GetString("ustand");
        }

        public override void acceptInput(string verb, Parser args)
        {
            base.acceptInput(verb, args);

            Sit sit = (Sit)parent.actions["sit"];
            if (client.Self.SittingOn == 0 && !sit.sittingOnGround)
            {
                parent.output("You are already standing.");
            }
            else
            {
                client.Self.Crouch(false);
                client.Self.Stand();
                parent.output("You stood up.");
                sit.sittingOnGround = false;
            }

            parent.describeNext = true;
        }
    }
}
