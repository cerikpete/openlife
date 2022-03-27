using Core;

namespace UI.Actions
{
    class Mute : Action
    {
        public Mute(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("smute");
            usageString = parent.RM.GetString("umute");
        }

        public override void acceptInput(string verb, Parser args)
        {
            base.acceptInput(verb, args);

            Avatar avatar;
			Listeners.Chat chat = (Listeners.Chat)parent.listeners["chat"];
			if (args.str=="all") {
			    chat.muted = true;  // inverse mute
				if (chat.muted) parent.output("All conversations muted");
				else parent.output("All conversations unmuted");
			} 
			else if (((Listeners.Avatars)parent.listeners["avatars"]).tryGetAvatar(args.str, out avatar))
            {
               // Listeners.Chat chat = (Listeners.Chat)parent.listeners["chat"];
                if (chat.muteList.Contains(avatar.Name))
                {
                    parent.output("Unmuted " + avatar.Name + ".");
                    chat.muteList.Remove(avatar.Name);
                }
                else
                {
                    parent.output("Muted " + avatar.Name + ".");
                    chat.muteList.Add(avatar.Name);
                }
            }
            else
            {
                parent.output("I don't know who " + args.str + " is.");
            }
        }
    }
}
