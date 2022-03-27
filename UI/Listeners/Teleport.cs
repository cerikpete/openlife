using Core;

namespace UI.Listeners
{
    public class Teleport : Listener
    {
        public Teleport(TextForm parent)
            : base(parent)
        {
            client.Self.OnTeleport += new AgentManager.TeleportCallback(Self_OnTeleport);
        }

        void Self_OnTeleport(string message, AgentManager.TeleportStatus status, AgentManager.TeleportFlags flags)
        {
            parent.describeNext = false;
            if (status == AgentManager.TeleportStatus.Failed)
            {
                parent.output("Teleport failed.");
                parent.describeSituation();
            }
            else if (status == AgentManager.TeleportStatus.Finished)
            {
                parent.output(message);
                parent.describePeople(false);
                parent.describeObjects(false);
                parent.describeBuildings(false);
                parent.timer1.Enabled = false;

                if (parent.currSim.Name != client.Network.CurrentSim.Name)
                {
                    // commentlogging Trace.WriteLine("\r\nSIM NAME: " + parent.currSim.Name.ToUpper());
                    // commentlogging Trace.WriteLine("NR OF PRIMS: " + parent.nrofprims + " NR OF OBJS: " + parent.nrofObjects);
                    // commentlogging Trace.WriteLine("Nr of good objects: " + parent.nrofgoodObjs + " Nr of bad objects: " + parent.nrofbadObjs);
                    // commentlogging if (parent.nrofgoodObjs != 0)
                    // commentlogging Trace.WriteLine("Avg good name length: " + parent.goodObjsCumulativeLength / parent.nrofgoodObjs + "\r\n");
                    parent.ClearData();
                    parent.currSim = client.Network.CurrentSim;
                    client.Appearance.SetPreviousAppearance(false);
                    //Ini inifile = new Ini(".\\textsl.ini");
                    parent.textSLini.IniWriteValue("Start", "Sim", client.Network.CurrentSim.Name);
                }
            }
        }

    }
}
