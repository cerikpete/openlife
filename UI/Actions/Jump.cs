namespace UI.Actions
{
    class Jump : Action
    {
        public Jump(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("sjump");
            usageString = parent.RM.GetString("ujump"); 
        }

        public override void acceptInput(string verb, Parser args)
        {
            base.acceptInput(verb, args);

            parent.output("You jumped.");
            client.Self.Jump();

            parent.describeNext = true;
        }
    }
}
