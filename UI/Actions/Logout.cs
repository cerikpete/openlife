namespace UI.Actions
{
    class Logout : Action
    {
		public Logout(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("slogout");
            usageString = parent.RM.GetString("ulogout");
        }

        public override void acceptInput(string verb, Parser args)
        {
            if (client.Network.Connected)
            {
                // commentlogging if (client.Network.CurrentSim != null)
                // commentlogging Trace.WriteLine("\r\nSIM NAME: " + client.Network.CurrentSim.Name.ToUpper());
                // commentlogging Trace.WriteLine("NR OF PRIMS: " + parent.nrofprims + " NR OF OBJS: " + parent.nrofObjects);
                // commentlogging Trace.WriteLine("Nr of good objects: " + parent.nrofgoodObjs + " Nr of bad objects: " + parent.nrofbadObjs);
                // commentlogging if (parent.nrofgoodObjs != 0)
                // commentlogging Trace.WriteLine("Avg good name length: " + parent.goodObjsCumulativeLength / parent.nrofgoodObjs + "\r\n");
                // commentlogging Trace.Flush();
                parent.ClearData();
                client.Network.Logout();
                parent.Close();
            }
        }

    }
}
