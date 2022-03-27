using System.IO;
using System.Xml;
using Core;
using XmlReader = UI.Utilities.XMLInterpreter.XmlReader;

namespace UI.Tutorials
{
    public class Tutorial
    {
        public XmlReader XMLTutorial;

        protected TextForm parent;
        protected SecondLife client;
        protected string helpString;
        protected string usageString;
        protected string AcceptableCommand= "";
        protected string FailureMessage = "Please try again!";
        protected string SuccessMessage = "Congrats! you could do it.";

        private int CommandCnt = 0;
        private int prevMode; 

        public Tutorial(string TutorialPath, TextForm _parent)
        {
            helpString = "No information for what this tutorial will teach.";
            usageString = "No usage instruction for this tutorial.";
            if (File.Exists(TutorialPath))
                XMLTutorial = new XmlReader(TutorialPath);
            else
            {
                CreateTutorial(TutorialPath);
                XMLTutorial = new XmlReader(TutorialPath);
            }            
            parent = _parent;
            client = parent.client;                   
        }

        public virtual string makeHelpString()
        {
            return helpString;
        }

        public virtual string makeUsageString()
        {
            return usageString;
        }

        public virtual void SetModeTutorial()
        {
            prevMode = parent.RunningMode;
            parent.RunningMode = (int)Modes.tutorial;
            CommandCnt = XMLTutorial.getAllChildren().Count;
        }

        public virtual void RestoreMode()
        {
            parent.RunningMode = prevMode;
        }

        public virtual int CommandCount
        {
            get { return CommandCnt; }
        }

        public virtual void ExecuteTutorial(string text)
        {
        }

        private void CreateTutorial(string TutorialPath)
        {
            XmlDocument docXML = new XmlDataDocument();

            docXML.LoadXml("<tutorial1>" +

"<command>" +
"<acceptable>help</acceptable>"+
"<instruction>Welcome to the tutorial for TextSL.You can type \"exit\" anytime to exit from tutorial. This tutorial teaches you how to move your avatar, how to find out what is around you, and how to interact with other avatars and objects using the commands that TextSL offers. The first command we are going to learn is \"help\". To get an overview of available commands, type in \"help\" </instruction>" +
"<failure>Please type in \"help\" .</failure>" +
"<success></success>" +
"</command>"+

"<command>"+
"<acceptable>help move</acceptable>"+
"<instruction>The help command can also be used to learn more about each command. Type \"help move\" to find out how to move.</instruction>"+
"<failure>Please type in \"help move\".</failure>"+
"<success></success>"+
"</command>"+

"<command>"+
"<acceptable>west</acceptable>"+
"<instruction>Ok, now try moving in the West direction. Please type \"west\" to do so." +
"</instruction>"+
"<failure>Please type in \"west\".</failure>"+
"<success>Correct! TextSL tells you when you cannot move into a particular direction. By default you move 5 meters.  </success>"+
"</command>"+

"<command>"+
"<acceptable>north 10</acceptable>"+
"<instruction>Ok, you can also move a variable distance. Try moving 10 meters to the North by typing \"north 10\".</instruction>"+
"<failure>Please type in \"north 10\".</failure>"+
"<success>Correct! When you move to a new location, TextSL automatically tells you how many objects and avatars are around you. You can also get this information by using the \"describe\" command. </success>"+
"</command>"+


"<command>"+
"<acceptable>describe</acceptable>"+
"<instruction>Please type \"describe\". </instruction>"+
"<failure>Please type in \"describe\".</failure>"+
"<success>This command tells you how many objects and avatars are around you.</success>"+
"</command>"+

"<command>"+
"<acceptable>describe objects</acceptable>"+
"<instruction>To get the names of the objects around you, type \"describe objects\".</instruction>"+
"<failure>Please type in \"describe objects\".</failure>"+
"<success>Good! If you want to know more about a certain object, type \"describe followed by the object's name\". </success>"+
"</command>"+

"<command>"+
"<acceptable>describe people</acceptable>"+
"<instruction>To get the names of the avatars around you type \"describe people\".</instruction>"+
"<failure>Please type in \"describe people\".</failure>"+
"<success>Good! When you want to know more about a certain avatar, type \"describe followed by the avatar's name\".</success>"+
"</command>"+


"<command>"+
"<acceptable>say</acceptable>"+
"<instruction>Use the \"say \" command to talk to other avatars. Â Just type \"say\" followed by your message.</instruction>"+
"<failure>Please type in \"say hello\".</failure>"+
"<success>You can also send private messages using the \"whisper\" command.</success>"+
"</command>"+

"<command>"+
"<acceptable>move to</acceptable>"+
"<instruction>When you know the name of an avatar or object you can also move to this object or avatar by typing \"move to\" followed by the name of the object or avatar. Now try to move to an object or an avatar.</instruction>"+
"<failure>Please type in \"move to\" followed by an object or an avatar name.</failure>"+
"<success>This concludes this tutorial. There are numerous other commands available that allow you to do such things as following other users or flying from one location to another. Type\"help\" to get an overview of all available commands and type \"help\" followed by the name of the command to learn more about each individual command. To quit TextSL, type \"quit\". This tutorial will now end. If you have any feedback please contact us through our website: http://www.textsl.org. We hope you enjoy Second Life using TextSL.</success>"+
"</command>"+

"</tutorial1>");

            docXML.Save(TutorialPath);            

        }

    }
}