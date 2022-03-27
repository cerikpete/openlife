namespace UI.Actions
{
    class Exit : Action
    {
        public Exit(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("sexit");
            usageString = parent.RM.GetString("uexit");
        }

        public override void acceptInput(string verb, Parser args)
        {
            base.acceptInput(verb, args);

            if (parent.RunningMode == (int)Modes.normal)
            {
                parent.logout();                
            }
            else
            {
                parent.RunningMode = (int)Modes.normal;
                parent.output("you are back to normal mode.");
            }

            parent.describeNext = false;
        }
    }
}
