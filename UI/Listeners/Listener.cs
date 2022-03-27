using Core;

namespace UI.Listeners
{
    public class Listener
    {
        protected TextForm parent;
        protected SecondLife client;

        public Listener(TextForm _parent)
        {
            parent = _parent;
            client = parent.client;
        }
    }
}
