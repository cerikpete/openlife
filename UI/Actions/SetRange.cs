namespace UI.Actions
{
    class SetRange : Action
    {
        public SetRange(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("srange");
            usageString = parent.RM.GetString("urange");            
        }

        public override void acceptInput(string verb, Parser args)
        {
            //Ini inifile = new Ini(".\\textsl.ini");
            base.acceptInput(verb, args);

            string[] tokens = args.objectPhrase.Split(null);
            if (tokens[0]=="")
                parent.output("range: " + parent.scanRange + " " + parent.objectsCap + " " + parent.avatarsCap );
            else if (tokens.Length == 1)
            {
                parent.scanRange = int.Parse(tokens[0]);
                parent.textSLini.IniWriteValue("Range", "Scan", tokens[0]);
            }
            else if (tokens.Length == 2)
            {
                parent.scanRange = int.Parse(tokens[0]);
                parent.objectsCap = int.Parse(tokens[1]);
                parent.textSLini.IniWriteValue("Range", "Scan", tokens[0]);
                parent.textSLini.IniWriteValue("Range", "Objects", tokens[1]);
            }
            else if (tokens.Length == 3)
            {
                parent.scanRange = int.Parse(tokens[0]);
                parent.objectsCap = int.Parse(tokens[1]);
                parent.avatarsCap = int.Parse(tokens[2]);
                parent.textSLini.IniWriteValue("Range", "Scan", tokens[0]);
                parent.textSLini.IniWriteValue("Range", "Objects", tokens[1]);
                parent.textSLini.IniWriteValue("Range", "Avs", tokens[2]);
            }

                

        }

    }
}
