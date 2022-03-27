using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core;
using UI.Utilities;

namespace UI
{
    public delegate void OutputDelegate(string str);
    public delegate void DescribeDelegate(bool detailed);
    enum Modes { normal, tutorial, chat };

    public partial class TextForm : Form
    {
        public SecondLife client;
        public ResourceManager RM;
        public Simulator currSim;
        public OutputDelegate outputDelegate;

        public Dictionary<string, DescribeDelegate> describers;

        public Dictionary<string, Listeners.Listener> listeners;
        public Dictionary<string, Actions.Action> actions;
        public Dictionary<string, Tutorials.Tutorial> tutorials;
        public Dictionary<string, Actions.Action> mainActions;

        public bool describeNext;
        private int describePos;
        private string currTutorial;
        public string outputStr;
        public String[] tutorialCommands;
        // commentprevnext public List<String> commandList;        
        // commentprevnext public int cmdIdx=0;

        public int scanRange = 30;
        public int objectsCap = 7;
        public int avatarsCap = 3;
        public int RunningMode = (int)Modes.normal;
        public int nrofprims = 0;
        public int nrofObjects = 0;
        public int nrofbadObjs = 0;
        public int nrofgoodObjs = 0;
        public int goodObjsCumulativeLength = 0;
        // commentlogging Stream logFile;
        Stream iniFile;
        public Ini textSLini;
        public string startLocation = "Virtual Ability";

        public TextForm()
        {
            client = new SecondLife();
            RM = new ResourceManager("UI.Actions.slStrings", Assembly.GetExecutingAssembly());

            if (File.Exists("textsl.ini"))
            {
                textSLini = new Ini(".\\textsl.ini");
                scanRange = int.Parse(textSLini.IniReadValue("Range", "Scan"));
                objectsCap = int.Parse(textSLini.IniReadValue("Range", "Objects"));
                avatarsCap = int.Parse(textSLini.IniReadValue("Range", "Avs"));
            }
            else
            {
                iniFile = File.Create("textsl.ini");
                iniFile.Close();
                textSLini = new Ini(".\\textsl.ini");

                textSLini.IniWriteValue("Login", "firstname", "");
                textSLini.IniWriteValue("Login", "lastname", "");
                textSLini.IniWriteValue("Login", "password", "");

                textSLini.IniWriteValue("Range", "Scan", scanRange.ToString());
                textSLini.IniWriteValue("Range", "Objects", objectsCap.ToString());
                textSLini.IniWriteValue("Range", "Avs", avatarsCap.ToString());

                textSLini.IniWriteValue("Start", "Sim", startLocation);
            }

            /*try
            {
                Assembly tsl = Assembly.Load("textsl");
                Stream resourceStr = tsl.GetManifestResourceStream("textsl.Actions.slStrings.resources");
                if (resourceStr == null)
                    throw new Exception("Could not locate embedded resource 'slStrings' in assmbly: 'textsl'");
            }
            catch (Exception e)
            {
                throw new Exception("textsl: " + e.Message);
            }*/

            client.Settings.ALWAYS_DECODE_OBJECTS = true;
            client.Settings.ALWAYS_REQUEST_OBJECTS = true;
            client.Settings.OBJECT_TRACKING = true;

            client.Network.OnConnected += new NetworkManager.ConnectedCallback(Network_OnConnected);
            client.Network.OnDisconnected += new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
            client.Network.OnLogin += new NetworkManager.LoginCallback(Network_OnLogin);
            startLocation = NetworkManager.StartLocation("Virtual Ability", 128, 127, 23);

            outputDelegate = new OutputDelegate(doOutput);

            // Create a file for output named TestFile.txt.
            // commentlogging if (File.Exists("textslLog_" + DateTime.Today.Month.ToString() + "_" + DateTime.Today.Day.ToString() + "_" + DateTime.Today.Year.ToString() + ".txt"))
            // commentlogging logFile = File.Open("textslLog_" + DateTime.Today.Month.ToString() + "_" + DateTime.Today.Day.ToString() + "_" + DateTime.Today.Year.ToString() + ".txt", FileMode.Append);
            // commentlogging else
            // commentlogging logFile = File.Create("textslLog_" + DateTime.Today.Month.ToString() + "_" + DateTime.Today.Day.ToString() + "_" + DateTime.Today.Year.ToString() +".txt");

            /* Create a new text writer using the output stream, and add it to
             * the trace listeners. */
            // commentlogging TextWriterTraceListener myTextListener = new
            // commentlogging TextWriterTraceListener(logFile);
            // commentlogging Trace.Listeners.Add(myTextListener);

            // Write output to the file.
            // commentlogging Trace.WriteLine("\"Text SL Log File\"\r\n");

            // Flush the output.
            // commentlogging Trace.Flush();

            tutorialCommands = ReadFromFile(Directory.GetParent(Directory.GetCurrentDirectory()) + "\\XMLTutorials\\commands.txt").Split(null);
            // commentprevnext commandList = new List<String>();

            describers = new Dictionary<string, DescribeDelegate>();
            describers["location"] = new DescribeDelegate(describeLocation);
            describers["people"] = new DescribeDelegate(describePeople);
            describers["objects"] = new DescribeDelegate(describeObjects);
            //describers["buildings"] = new DescribeDelegate(describeBuildings);

            describePos = 0;

            listeners = new Dictionary<string, Listeners.Listener>();
            listeners["chat"] = new Listeners.Chat(this);
            listeners["avatars"] = new Listeners.Avatars(this);
            listeners["teleport"] = new Listeners.Teleport(this);
            listeners["whisper"] = new Listeners.Whisper(this);
            listeners["objects"] = new Listeners.Objects(this);
            listeners["bump"] = new Listeners.Bump(this);
            listeners["sound"] = new Listeners.Sound(this);


            actions = new Dictionary<string, Actions.Action>();
            mainActions = new Dictionary<string, Actions.Action>();


            actions["login"] = new Actions.Login(this);
            actions["describe"] = new Actions.Describe(this);
            mainActions["describe"] = actions["describe"];
            actions["say"] = new Actions.Say(this);
            mainActions["say"] = actions["say"];
            actions["whisper"] = new Actions.Whisper(this);
            mainActions["whisper"] = actions["whisper"];
            actions["mute"] = new Actions.Mute(this);
            mainActions["mute"] = actions["mute"];
            actions["sit"] = new Actions.Sit(this);
            mainActions["sit"] = actions["sit"];
            actions["stand"] = new Actions.Stand(this);
            mainActions["stand"] = actions["stand"];
            //actions["jump"] = new Actions.Jump(this);
            //actions["crouch"] = new Actions.Crouch(this);
            Actions.Move move = new Actions.Move(this);
            actions["move"] = move;
            mainActions["move"] = actions["move"];
            actions["west"] = move;
            actions["east"] = move;
            actions["north"] = move;
            actions["south"] = move;
            actions["fly"] = new Actions.Fly(this);
            mainActions["fly"] = actions["fly"];
            //actions["stop-flying"] = new Actions.StopFlying(this);
            Actions.Follow follow = new Actions.Follow(this);
            actions["follow"] = follow;
            mainActions["follow"] = actions["follow"];
            //actions["stop-following"] = follow;

            //actions["where"] = new Actions.Where(this);
            actions["teleport"] = new Actions.Teleport(this);
            mainActions["teleport"] = actions["teleport"];
            //actions["locate"] = new Actions.Locate(this);

            actions["stop"] = new Actions.Stop(this);
            mainActions["stop"] = actions["stop"];
            actions["help"] = new Actions.Help(this);
            mainActions["help"] = actions["help"];
            actions["quit"] = new Actions.Logout(this);
            mainActions["quit"] = actions["quit"];

            actions["reset"] = new Actions.Reset(this);
            actions["range"] = new Actions.SetRange(this);
            tutorials = new Dictionary<string, Tutorials.Tutorial>();
            tutorials["tutorial"] = new Tutorials.Tutorial1(this);
            //tutorials["test1"] = new Tutorials.Test1(this);
            actions["exit"] = new Actions.Exit(this);

            describeNext = true;
            InitializeComponent();

            WindowState = FormWindowState.Maximized;
            consoleInputText.Enabled = true;
            consoleInputText.Focus();

            if ((textSLini.IniReadValue("Login", "firstname") != "") && (textSLini.IniReadValue("Login", "lastname") != "") && (textSLini.IniReadValue("Login", "password") != ""))
                doOutput("Trying to login, please wait..");
        }

        public static string ReadFromFile(string filename)
        {
            if (File.Exists(filename))
            {
                StreamReader SR;
                string S;
                SR = File.OpenText(filename);
                S = SR.ReadLine();
                SR.Close();
                return S;
            }
            else
                return "";
        }

        void Network_OnDisconnected(NetworkManager.DisconnectType reason, string message)
        {
            if (message.Length > 0)
                output("Disconnected from server. Reason is " + message + ". " + reason);
            else
                output("Disconnected from server. " + reason);

            output("Please restart Second Life.");
        }

        public void ClearData()
        {
            Listeners.Objects objects = (Listeners.Objects)listeners["objects"];
            objects.ClearPrims();
            Listeners.Avatars avs = (Listeners.Avatars)listeners["avatars"];
            avs.ClearAvs();
            nrofObjects = 0;
            nrofprims = 0;
            nrofgoodObjs = 0;
            nrofbadObjs = 0;
            goodObjsCumulativeLength = 0;
        }

        void Network_OnConnected(object sender)
        {
            consoleInputText.Focus();

            System.Threading.Thread.Sleep(1000);
            describeAll();
        }

        public void Network_OnLogin(LoginStatus login, string message)
        {
            if (login == LoginStatus.Failed)
                output("Not able to login");
            else if (login == LoginStatus.Success)
            {

                client.Appearance.SetPreviousAppearance(false);
                output("Welcome " + client.Self.FirstName + " " + client.Self.LastName + "!");
                output("You have logged in successfully. You can take our tutorial anytime by typing \"tutorial\"");
                //Ini inifile = new Ini(".\\textsl.ini");
                textSLini.IniWriteValue("Start", "Sim", client.Network.CurrentSim.Name);
                currSim = client.Network.CurrentSim;
                // commentlogging Trace.WriteLine("Avatar " + client.Self.Name + " logged in at " + DateTime.Now.ToString() + "\r\n");
                // commentlogging Trace.Flush();
            }
        }

        private void TextForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            logout();
            // commentlogging logFile.Close();            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //LoginForm loginForm = new LoginForm(client);
            //loginForm.ShowDialog();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logout();
        }

        public void logout()
        {
            ExecuteCommand("quit");
            /*if (client.Network.Connected)
            {
                if (client.Network.CurrentSim != null)
                    Trace.WriteLine("\r\nSIM NAME: " + client.Network.CurrentSim.Name.ToUpper());
                Trace.WriteLine("NR OF PRIMS: " + nrofprims + " NR OF OBJS: " + nrofObjects);
                Trace.WriteLine("Nr of good objects: " + nrofgoodObjs + " Nr of bad objects: " + nrofbadObjs);
                if (nrofgoodObjs != 0)
                    Trace.WriteLine("Avg good name length: " + goodObjsCumulativeLength / nrofgoodObjs + "\r\n");
                Trace.Flush();
                ClearData();
                client.Network.Logout();                
            }*/
        }

        public void output(string str)
        {
            this.Invoke(outputDelegate, str);
        }

        public void doOutput(string str)
        {
            consoleText.AppendText(str + "\r\n");

            string oldInput = consoleInputText.Text;
            consoleInputText.Clear();
            consoleInputText.Text = str;
            // commentlogging Trace.WriteLine("OUTPUT: " + str + "    " + DateTime.Now.ToString());
            // commentlogging Trace.Flush();
            consoleInputText.SelectAll();
            System.Threading.Thread.Sleep(100);
            consoleInputText.Text = oldInput;
        }

        public void acceptConsoleInput()
        {
            Char[] quote = new char[2];
            quote[0] = '/';
            quote[1] = '"';
            if ((!client.Network.Connected) && (!consoleInputText.Text.ToLower().Contains("login")))
            {
                doOutput("You need to login to access Second Life");
                consoleInputText.Text = "";
            }
            else
            {
                string text = consoleInputText.Text;

                // commentlogging Trace.WriteLine("INPUT: " + text + "    " + DateTime.Now.ToString());
                // commentlogging Trace.Flush();
                //output(text);
                // commentprevnext commandList.Add(text);
                // commentprevnext cmdIdx = commandList.Count - 1;

                if (text.StartsWith("\""))
                {
                    text = text.TrimStart(quote);
                    text = text.TrimEnd(quote);
                    text = "say " + text;
                }
                // commentchatmode else if ((RunningMode == (int)Modes.chat)&&(text!="exit"))
                // commentchatmode text = "say " + text;

                // commentchatmode if (text == "chat")
                // commentchatmode RunningMode = (int)Modes.chat;

                consoleInputText.Text = "";
                string verb = text.Split(null)[0].ToLower();

                if (text.Length > 0)
                {
                    if (RunningMode == (int)Modes.tutorial)
                    {
                        describeNext = false;
                        tutorials[currTutorial].ExecuteTutorial(text);
                    }
                    else if ((RunningMode == (int)Modes.normal))// commentchatmode ||((RunningMode == (int)Modes.chat)))
                    {
                        if (tutorials.ContainsKey(verb))
                        {
                            currTutorial = verb;
                            describeNext = false;
                            tutorials[verb].ExecuteTutorial(text);
                        }
                        // commentchatmode else if (text == "chat")
                        // commentchatmode {
                        // commentchatmode output("you have entered chat mode, you can type exit anytime.");
                        // commentchatmode }
                        else
                            ExecuteCommand(text);
                    }

                    if ((client.Network.Connected) && (describeNext))
                    {
                        describeNext = false;
                        describeSituation();
                    }
                }
            }
        }

        public bool ExecuteCommand(string text)
        {
            string verb = text.Split(null)[0].ToLower();

            if (actions.ContainsKey(verb))
            {
                if (text.Length > verb.Length)
                    actions[verb].acceptInputWrapper(verb, text.Substring(verb.Length + 1));
                else
                    actions[verb].acceptInputWrapper(verb, "");
                return true;
            }
            else
            {
                output("I don't understand the verb " + verb + ".");
                output("Type \"help\" for help.");
                describeNext = true;

                return false;
            }
        }

        private void consoleInputText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 13)
                submitButton_Click(sender, e);
        }

        public void describeAll()
        {
            foreach (string dname in describers.Keys)
                describers[dname].Invoke(false);
        }

        public void describeSituation()
        {
            int i = 0;
            string name = "";
            foreach (string dname in describers.Keys)
                if (i++ == describePos)
                    name = dname;
            describePos = (describePos + 1) % describers.Count;
            describers[name].Invoke(false);
        }

        public void describeLocation(bool detailed)
        {
            output("You are in " + client.Network.CurrentSim.Name + " (" + ((int)client.Self.SimPosition.X).ToString() + "," + ((int)client.Self.SimPosition.Y).ToString() + "," + ((int)client.Self.SimPosition.Z).ToString() + ").");
        }

        public void describePeople(bool detailed)
        {
            Listeners.Avatars avatars = (Listeners.Avatars)listeners["avatars"];
            if (detailed)
            {
                List<Avatar> avatarList = avatars.getAvatarsNear(client.Self.RelativePosition, avatarsCap);
                if (avatarList.Count > 1)
                {
                    string str = "You see the people ";
                    for (int i = 0; i < avatarList.Count - 1; ++i)
                        str += avatars.getAvatarName(avatarList[i]) + ", ";
                    str += "and " + avatars.getAvatarName(avatarList[avatarList.Count - 1]) + ".";
                    output(str);
                }
                else if (avatarList.Count == 1)
                {
                    output("You see 1 person: " + avatars.getAvatarName(avatarList[0]) + ".");
                }
                else
                    output("You don't see anyone around.");
            }
            else
            {
                if (avatars.numAvatars() > 2)
                {
                    int avcnt = avatars.numAvatars() - 1;
                    output("You see " + avcnt + " people.");
                }
                else if (avatars.numAvatars() == 2)
                    output("You see " + 1 + " person.");
                else
                    output("You don't see anyone around.");
            }
        }

        public void describeObjects(bool detailed)
        {
            Listeners.Objects objects = (Listeners.Objects)listeners["objects"];
            List<Primitive> prims = objects.getPrimitives(objectsCap);
            if (detailed)
            {
                if (prims.Count > 1)
                {
                    string str = "You see the objects ";
                    for (int i = 0; i < prims.Count - 1; ++i)
                        str += objects.getObjectName(prims[i]) + ", ";
                    str += "and " + objects.getObjectName(prims[prims.Count - 1]) + ".";
                    output(str);
                }
                else if (prims.Count == 1)
                {
                    output("You see one object: " + objects.getObjectName(prims[0]));
                }
                else
                {
                    output("You don't see any objects around.");
                }
            }
            else
            {
                if (prims.Count == 1)
                    output("You see 1 object");
                else if (prims.Count > 1)
                    output("You see " + prims.Count + " objects");
                //else if (prims.Count >= 3)
                //output("You see many objects");
                else
                    output("You don't see any objects around.");
            }
        }

        public void describeBuildings(bool detailed)
        {
            /*
            List<LLVector3> buildings = ((Listeners.Objects)listeners["objects"]).getBuildings(3);
            output("You see " + buildings.Count + " buildings.");
              */
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            acceptConsoleInput();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (RunningMode == (int)Modes.tutorial)
            {
                timer1.Enabled = false;
                Tutorials.Test1 test = (Tutorials.Test1)tutorials["test1"];
                test.UpdateHits();
            }
            else
            {
                timer1.Enabled = false;
                output("Unable to teleport to the location you mentioned.");
            }
        }

        private void consoleText_TextChanged(object sender, EventArgs e)
        {
            outputStr = consoleText.Lines[consoleText.Lines.Length - 2];
            if ((timer1.Enabled) && (RunningMode == (int)Modes.tutorial))
            {
                Tutorials.Test1 test = (Tutorials.Test1)tutorials["test1"];
                test.UpdateHits();
            }
        }

        private void consoleInputText_KeyDown(object sender, KeyEventArgs e)
        {
            /* commentprevnext
            if (Convert.ToInt32(e.KeyValue) == 38)
            {
                if ((cmdIdx < commandList.Count)&&(cmdIdx>=0))
                {
                    consoleInputText.Text = commandList[cmdIdx];
                    if (cmdIdx!=0)
                        cmdIdx--;
                }
            }
            else if (Convert.ToInt32(e.KeyValue) == 40)
            {
                if ((cmdIdx < commandList.Count) && (cmdIdx >= 0))
                {
                    consoleInputText.Text = commandList[cmdIdx];
                    if (cmdIdx != commandList.Count - 1)
                        cmdIdx++;
                }
            } */
        }

        private void TextForm_Shown(object sender, EventArgs e)
        {
            ExecuteCommand("login");
        }
    }
}
