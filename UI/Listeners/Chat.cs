using System.Collections.Generic;
using Core;

namespace UI.Listeners
{
    public class Chat : Listener
    {
        public List<string> muteList;
		public bool muted = false;

        public Chat(TextForm parent) : base(parent) {
            muteList = new List<string>();

            client.Self.OnChat += new AgentManager.ChatCallback(Self_OnChat);
        }

        void Self_OnChat(string message, ChatAudibleLevel audible, ChatType type,
            ChatSourceType sourceType, string fromName, LLUUID id, LLUUID ownerid,
            LLVector3 position)
        {
            if (message.Length > 0 && sourceType == ChatSourceType.Agent && !muteList.Contains(fromName) && !muted)
            {
                parent.output(fromName + " says, \"" + message + "\".");
            }
            else if (sourceType != ChatSourceType.Agent)
                parent.output(fromName + " says, \"" + message + "\".");
        }
    }
}
