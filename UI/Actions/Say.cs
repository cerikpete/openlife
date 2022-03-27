using Core;

namespace UI.Actions
{
    class Say : Action
    {
        public Say(TextForm parent) 
            : base(parent) 
        {
            helpString = parent.RM.GetString("ssay");
            usageString = parent.RM.GetString("usay");
        }

        public override void acceptInput(string verb, Parser args)
        {
            base.acceptInput(verb, args);

            if (args.str.Length > 0)
            {
                client.Self.Chat(args.str, 0, ChatType.Normal);
            }
        }
    }
}
