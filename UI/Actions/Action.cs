using Core;

namespace UI.Actions
{
    public class Action
    {
        protected TextForm parent;
        protected SecondLife client;
        protected string helpString;
        protected string usageString;

        public Action(TextForm _parent)
        {
            helpString = "No help information for this action.";
            usageString = "No usage instruction for this action.";

            parent = _parent;
            client = parent.client;
        }

        public void acceptInputWrapper(string verb, string args)
        {
            acceptInput(verb, new Parser(args));
        }

        public virtual void acceptInput(string verb, Parser args)
        {
        }

        public virtual string makeHelpString()
        {
            return helpString;
        }

        public virtual string makeUsageString()
        {
            return usageString;
        }
    }
}
