using Core;

namespace UI.Actions
{
    class Whisper : Action
    {
        public LLUUID currentAvatar;
        public LLUUID currentSession;

        public Whisper(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("swhisper");
            usageString = parent.RM.GetString("uwhisper");

            currentAvatar = null;
            currentSession = null;
        }

        public override void acceptInput(string verb, Parser args)
        {
            int i =0;
            base.acceptInput(verb, args);

            //string to = args.prepPhrases["to"];
            string[] to = args.prepPhrases["to"].Split(null);

            if (to.ToString() != "") {
                Avatar avatar;
                Listeners.Avatars avatars = (Listeners.Avatars)parent.listeners["avatars"];
                if (to.Length >= 2)
                {
                    if (avatars.tryGetAvatar(to[0] + " " + to[1], out avatar))
                    {
                        currentAvatar = avatar.ID;
                        for (i = 2; i < to.Length; i++)
                            args.objectPhrase = args.objectPhrase + " " + to[i];
                        args.objectPhrase = args.objectPhrase.Trim();
                    }
                    else if (avatars.tryGetAvatar(to[0], out avatar))
                    {
                        currentAvatar = avatar.ID;
                        for (i = 1; i < to.Length; i++)
                            args.objectPhrase = args.objectPhrase + " " + to[i];
                        args.objectPhrase = args.objectPhrase.Trim();
                    }
                    else
                    {
                        parent.output("I don't know who " + to[0] + " is.");
                        return;
                    }
                }
                else if (to.Length == 1)
                {
                    if (avatars.tryGetAvatar(to[0], out avatar))
                    {
                        currentAvatar = avatar.ID;
                    }
                    else
                    {
                        parent.output("I don't know who " + to + " is.");
                        return;
                    }
                }


                //currentAvatar = avatar.ID;
            }
            else if (currentAvatar == null)
            {
                parent.output("Please provide a name to whisper to.");
                return;
            }

            if (currentSession != null)
                client.Self.InstantMessage(currentAvatar, args.objectPhrase, currentSession);
            else
                client.Self.InstantMessage(currentAvatar, args.objectPhrase);
        }
    }
}
