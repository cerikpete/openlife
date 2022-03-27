namespace UI.Actions
{
    class Locate : Action
    {
        public Locate(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("slocate");
            usageString = parent.RM.GetString("ulocate");
        }

        public override void acceptInput(string verb, Parser args)
        {
            base.acceptInput(verb, args);

            parent.output("You are logged in as " + client.Self.FirstName + " " + client.Self.LastName + ", currently in " + client.Network.CurrentSim.Name + " " + (int)client.Self.SimPosition.X + "," + (int)client.Self.SimPosition.Y + "," + (int)client.Self.SimPosition.Z);
        }
    }
}
