using UI.Utilities.XMLInterpreter;

namespace UI.Tutorials
{
    class Tutorial1 : Tutorial
    {
        private int CommandIdx = 0;
        
        public Tutorial1(TextForm parent): base("tutorial1.xml", parent)
        {
            //Directory.GetParent(Directory.GetCurrentDirectory()) + "\\XMLTutorials\\tutorial1.xml"
            helpString = parent.RM.GetString("stutorial1");
            usageString = parent.RM.GetString("ututorial1");
        }

        private void SetNextCommand()
        {
            XmlReader XMLCommand = XMLTutorial.getAllChildren()[CommandIdx];
            parent.output(XMLCommand["instruction"]);
            AcceptableCommand = XMLCommand["acceptable"];
            FailureMessage = XMLCommand["failure"];
            SuccessMessage = XMLCommand["success"];
            parent.describeNext = false;       
        }

        public override void ExecuteTutorial(string text)
        {          
            if (CommandIdx == 0)
            {
                SetModeTutorial();
                parent.actions["mute"].acceptInputWrapper("mute", "all");
                //parent.output("Welcome to your first Tutorial! You can type \"exit\" anytime to go back to default mode.");                
                
                SetNextCommand();
                CommandIdx++;
            }
            else
            {
                string command = text.Split(null)[0].ToLower();
                //(AcceptableCommand.ToLower() == text.ToLower()) || (AcceptableCommand == command)
                if ((text.StartsWith(AcceptableCommand)) || (text.ToLower() == "exit") || (command == "help"))
                {
                    if (!parent.actions.ContainsKey(command))
                    {
                        parent.output(SuccessMessage);
                        if (CommandIdx < CommandCount)
                            SetNextCommand();
                        CommandIdx++;
                        if (CommandIdx == CommandCount + 1)
                        {
                            RestoreMode();
                            CommandIdx = 0;
                            parent.output("Congratulations!! You completed your first Tutorial! ");
                            parent.output("You can type \"help\" anytime to get the list of commands and any help on them. You can explore on your own now! Hope you will enjoy!");
                            parent.actions["stop"].acceptInputWrapper("", "mute all");
                        }                        
                    }
                    else if (parent.ExecuteCommand(text))
                    {
                        if (text == "exit")
                        {
                            RestoreMode();
                            CommandIdx = 0;
                            parent.actions["stop"].acceptInputWrapper("", "mute all");
                        }
                        else if ((command=="help") && (AcceptableCommand.Split(null)[0] !="help"))
                        {

                        }
                        else
                        {
                            parent.output(SuccessMessage);
                            if (text.StartsWith("stop mute all"))
                            {
                                parent.actions["mute"].acceptInputWrapper("mute", "all");
                                parent.output("Muting everyone again so that you can continue with tutorial without disturbance.");
                            }
                            if (CommandIdx < CommandCount)
                                SetNextCommand();
                            CommandIdx++;
                            if (CommandIdx == CommandCount + 1)
                            {
                                RestoreMode();
                                CommandIdx = 0;
                                //parent.output("Congratulations!! You completed your first Tutorial! ");
                                //parent.output("You can type \"help\" anytime to get the list of commands and any help on them. You can explore on your own now! Hope you will enjoy!");
                                parent.actions["stop"].acceptInputWrapper("", "mute all");
                            }
                        }
                    }
                }
                else
                    parent.output(FailureMessage);
            }
        }

    }
}
