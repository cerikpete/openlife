namespace UI.Actions
{
    class Stop : Action
    {
        public Stop(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("sstop");
			usageString = parent.RM.GetString("ustop");
        }

        public override void acceptInput(string verb, Parser args)
        {
            base.acceptInput(verb, args);
            string[] tokens = args.objectPhrase.Split(null);

            if (args.objectPhrase.Length == 0)
            {
                parent.output("Argument Expected. Please type \"help stop\" to know more about it");
            }
            else if (tokens[0] == "following")
            {
                parent.actions["follow"].acceptInputWrapper("stop-following", "");
            }
            else if (tokens[0] == "flying")
            {
                parent.output("You stopped flying.");
                client.Self.Fly(false);                
            }
            else if (tokens[0] == "mute")
            {
                Listeners.Chat chat = (Listeners.Chat)parent.listeners["chat"];
                if (tokens.Length == 2)
                {
                    if (tokens[1] == "all")
                    {
                        chat.muted = false;
                        if (chat.muted) parent.output("All conversations muted");
                        else parent.output("All conversations unmuted");
                    }
                    else
                        parent.actions["mute"].acceptInputWrapper("mute", tokens[1]);
                }
                else if (tokens.Length > 2)
                {
                    string avStr = "";
                    for (int i = 1; i < tokens.Length; i++)
                        avStr = avStr + tokens[i] + " ";
                    avStr = avStr.Trim();
                    parent.actions["mute"].acceptInputWrapper("mute", avStr);
                }
                else
                    parent.output("I don't know whom to stop mute for");                
            }

            parent.describeNext = false;
        }
    }
}
