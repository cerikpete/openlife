namespace UI.Actions
{
    class Crouch : Action
    {
        public Crouch(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("scrouch");
            usageString = parent.RM.GetString("ucrouch");
        }

        public override void acceptInput(string verb, Parser args)
        {
            base.acceptInput(verb, args);

            parent.output("You crouched.");
            client.Self.Crouch(true);

            parent.describeNext = true;
        }
    }
}
