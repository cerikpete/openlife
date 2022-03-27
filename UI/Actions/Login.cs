using UI.Utilities;

namespace UI.Actions
{
    class Login : Action
    {
		protected string firstName = "";
		protected string lastName = "";
		protected string password = "";
        
		public Login(TextForm parent)
            : base(parent)
        {
            helpString = parent.RM.GetString("slogin");
            usageString = parent.RM.GetString("ulogin");            
        }

		public override void acceptInput(string verb, Parser args)
        {
			base.acceptInput(verb, args);
            string[] tokens = args.objectPhrase.Split(null);
            //Ini inifile = new Ini(".\\textsl.ini");
            if (tokens[0] =="")
            {
                firstName = parent.textSLini.IniReadValue("Login", "firstname");
                lastName = parent.textSLini.IniReadValue("Login", "lastname");
                password = parent.textSLini.IniReadValue("Login", "password");    
                if (password!="")
                    password = Crypt.crypt(password, "abcd0123", false);

                if (!client.Network.Connected)
                {
                    if ((firstName != "") && (lastName != "") && (password != ""))
                        client.Network.Login(firstName, lastName, password, "TextSL", "UNR");
                    else
                        parent.output("Please login with your login credentials.\r\nTo do this, please type \"login firstname lastname password\"");
                }
                else
                    parent.output("You are already logged in.");
            }
            else if (tokens.Length != 3)
            {
                parent.output("Please enter login FirstName LastName and Password to login to the SL");
            }
            else
            {
                firstName = tokens[0];
                lastName = tokens[1];
                password = tokens[2];

                if (!client.Network.Connected)
                {
                    //string startLocation = "Healthinfo Island";
                    if (parent.textSLini.IniReadValue("Start", "Sim") == "Virtual Ability")
                        client.Network.Login(firstName, lastName, password, "TextSL", parent.startLocation, "UNR");
                    else
                        client.Network.Login(firstName, lastName, password, "TextSL", "UNR");
                    parent.textSLini.IniWriteValue("Login", "firstname", firstName);
                    parent.textSLini.IniWriteValue("Login", "lastname", lastName);
                    parent.textSLini.IniWriteValue("Login", "password", Crypt.crypt(password, "abcd0123", true));
                }
                else
                    parent.output("You are already logged in.");
            }
		}
    }
}