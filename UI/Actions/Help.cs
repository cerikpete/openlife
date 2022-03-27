namespace UI.Actions
{
    class Help : Action
    {
        public Help(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("shelp");
            usageString = parent.RM.GetString("uhelp");
        }

        public override void acceptInput(string verb, Parser args)
        {
            base.acceptInput(verb, args);

            if (args.objectPhrase.Length == 0)
            {
                //if (parent.RunningMode == (int)Modes.normal)
               // {
                    parent.output("Here is the list of commands: ");
                    foreach (string action in parent.mainActions.Keys)
                    {
                        parent.output(action + " " );
                    }
                    foreach (string tutorial in parent.tutorials.Keys)
                    {
                        parent.output(tutorial + " ");
                    }
                    parent.output("To know more about them you can type \"help command name\"");                    
                    // commentchatmode parent.output("chat: " + parent.RM.GetString("schat"));
                //}
                /*else
                {
                    foreach (string action in parent.tutorialCommands)
                    {
                        parent.output(action + ": " + parent.actions[action].makeHelpString());
                    }
                }*/
            }
            else
            {
                if (parent.actions.ContainsKey(args.objectPhrase))
                    parent.output(parent.actions[args.objectPhrase].makeUsageString());
                else if (parent.tutorials.ContainsKey(args.objectPhrase))
                    parent.output(parent.tutorials[args.objectPhrase].makeUsageString());
                // commentchatmode else if (args.objectPhrase == "chat")
                // commentchatmode parent.output(parent.RM.GetString("uchat"));
                else
                    parent.output("I don't know about the command " + args.objectPhrase + ".");
            }

            parent.describeNext = false;
        }
    }
}
