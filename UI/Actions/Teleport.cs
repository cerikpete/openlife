using System;
using Core;

namespace UI.Actions
{
    class Teleport : Action
    {        
        public Teleport(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("steleport");
            usageString = parent.RM.GetString("uteleport");
        }

        public override void acceptInput(string verb, Parser args)
        {
            string temp;

            base.acceptInput(verb, args);

            string[] tokens = args.prepPhrases["to"].Split(null);
            if (tokens.Length == 0)
            {
                parent.output("Provide somewhere to teleport to.");
            }
            else
            {
                string to ="";
                int X =128, Y=128, Z=27;
                bool ifCoordinates = false;

                if (tokens.Length > 3)
                {
                    try
                    {
                        X = int.Parse(tokens[tokens.Length - 3]);
                        Y = int.Parse(tokens[tokens.Length - 2]);
                        Z = int.Parse(tokens[tokens.Length - 1]);
                        ifCoordinates = true;
                    }
                    catch (Exception e) { temp = e.Message; }
                }

                if (!ifCoordinates)
                {
                    for (int i = 0; i < tokens.Length; i++)
                        to += tokens[i] + " ";
                    to = to.Trim();
                }
                else
                {
                    for (int i = 0; i < tokens.Length - 3; i++)
                        to += tokens[i] + " ";
                    to = to.Trim();
                }
                
                parent.output("Trying to teleport to " + to + ".");
                client.Self.Teleport(to, new LLVector3(X, Y, Z));
                parent.timer1.Interval = 1 * 60 * 1000;
                parent.timer1.Enabled = true;
            }

            parent.describeNext = false;
        }
    }
}
