namespace UI.Actions
{
    class StopFlying : Action
    {
        public StopFlying(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("sstopflying");
            usageString = parent.RM.GetString("ustopflying");
        }

        public override void acceptInput(string verb, Parser args)
        {
            base.acceptInput(verb, args);

            parent.output("You stopped flying.");
            client.Self.Fly(false);

            parent.describeNext = true;
        }
    }
}
