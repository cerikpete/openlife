namespace UI.Actions
{
    class Reset :Action
    {
		protected string firstName = "";
		protected string lastName = "";
		protected string password = "";
        
		public Reset(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("sreset");
            usageString = parent.RM.GetString("ureset");            
        }

		public override void acceptInput(string verb, Parser args)
        {
			base.acceptInput(verb, args);

            //Ini inifile = new Ini(".\\textsl.ini");
            parent.textSLini.IniWriteValue("Login", "firstname", "");
            parent.textSLini.IniWriteValue("Login", "lastname", "");
            parent.textSLini.IniWriteValue("Login", "password", "");

            parent.output("User login info reset");
            parent.describeNext = false;
		}
 
    }
}
