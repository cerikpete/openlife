using System.Collections.Generic;
using System.IO;
using UI.Utilities.XMLInterpreter;

namespace UI.Tutorials
{
    class Test1 : Tutorial
    {
        private int CommandIdx = 0;
        private int hitcount = 0;
        private int init = 0;
        protected string AutoRun = "false";
        protected string timeout = "2";
        protected string[] SuccessCriteria = {""};
        public List<string> currSuccessStrings;
        

        public Test1(TextForm parent) : base(Directory.GetParent(Directory.GetCurrentDirectory()) + "\\XMLTutorials\\test1.xml", parent)
        {
            helpString = parent.RM.GetString("stest1");
            usageString = parent.RM.GetString("utest1");
            currSuccessStrings = new List<string>();            
        }

        public int NfOfHits
        {
            get { return hitcount; }
        }

        private void SetNextCommand()
        {
            if (CommandIdx < CommandCount)
            {            
                XmlReader XMLCommand = XMLTutorial.getAllChildren()[CommandIdx];

                AutoRun = XMLCommand["autorun"];
                timeout = XMLCommand["timeout"];
                SuccessMessage = XMLCommand["success"];
                FailureMessage = XMLCommand["failure"];
                SuccessCriteria = XMLCommand["successcriteria"].Split(null);

                hitcount = 0;
                currSuccessStrings.Clear();
                foreach (string criteria in SuccessCriteria)            
                    currSuccessStrings.Add(criteria);
                parent.describeNext = false;

                if (AutoRun == "false")
                {
                    parent.output(XMLCommand["instruction"]);
                    parent.output("You have " + timeout + " mins to complete this task.");
                    parent.timer1.Interval = int.Parse(timeout) * 60 * 1000;
                    parent.timer1.Enabled = true;
                }            
                else
                {
                    int trials = 0;
                    while (trials < 1)
                    {
                        int i = 0;
                        parent.ExecuteCommand(XMLCommand["instruction"]);
                        System.Threading.Thread.Sleep(4000);
                        while (i < currSuccessStrings.Count)
                        {
                            if (parent.outputStr.Contains(currSuccessStrings[i]))
                            {
                                currSuccessStrings.RemoveAt(i);
                                hitcount++;
                            }
                            else
                                i++;
                        }

                        if (hitcount == SuccessCriteria.Length)
                            break;
                        trials++;
                    }
                    
                    CommandIdx++;                    
                    SetNextCommand();
                }
            }
            else
            {
                RestoreMode();
                CommandIdx = 0;
                init = 0;
                //parent.output("Congratulations!! You completed your first test! ");
                parent.actions["mute"].acceptInputWrapper("mute", "all");
            }            
        }

        private void CheckSuccess()
        {
            if (hitcount == SuccessCriteria.Length)
            {
                parent.timer1.Enabled = false;
                parent.output(SuccessMessage);
                CommandIdx++;
                SetNextCommand();
            }
            else
            {
                if (!parent.timer1.Enabled)
                {
                    parent.output(FailureMessage);
                    CommandIdx++;
                    SetNextCommand();
                }
            }
        }

        public void UpdateHits()
        {
            int i = 0;

            if (parent.timer1.Enabled)
            {
                while (i < currSuccessStrings.Count)
                {
                    if (parent.outputStr.Contains(currSuccessStrings[i]))
                    {
                        currSuccessStrings.RemoveAt(i);
                        hitcount++;
                    }
                    else
                        i++;
                }
            }
            CheckSuccess();
        }

        public override void ExecuteTutorial(string text)
        {
            if (init==0)
            {
                SetModeTutorial();

                //parent.output("Test time! Welcome to your first test. Here you will be provided with 4 tasks. Feel free to type help anytime during the test.");
                parent.actions["mute"].acceptInputWrapper("mute", "all");

                SetNextCommand();
                init = 1;
            }
            else if (CommandIdx < CommandCount)
            {
                parent.ExecuteCommand(text);
                if (text == "exit")
                {
                    RestoreMode();
                    CommandIdx = 0;
                    init = 0;
                    parent.actions["mute"].acceptInputWrapper("mute", "all");
                }
            }
            else
            {
                RestoreMode();
                CommandIdx = 0;
                parent.actions["mute"].acceptInputWrapper("mute", "all");                
            }
        }
    }
}
