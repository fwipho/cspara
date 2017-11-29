using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 1.0.0 SNJW 2017-11-26 a few classes for managing console parameters
/// 
/// This came about because I really liked how simple the dotnet.exe and node.exe command line tools worked.
/// I wanted to have a flexible version of that for .NET console applications.
/// I also want it to automatically generate the "help" functionality as well.
/// 
/// For those not familiar with the dotnet.exe (you probably should be if you're reading this though), 
/// it has a heirachical command system: you can type "dotnet {command} {commandoption}" to set {commandoption}.
/// (where {commandoption} is specific to that command).  
/// You can also have root-level commands: "dotnet --version"
/// Some switches are the same for each level: "dotnet publish --help"
/// 

//C:\Users\simon>dotnet
//Usage: dotnet [options]
//Usage: dotnet [path - to - application]
//Options:
//  -h|--help Display help.
//  --version Display version.
//
//path-to-application:
//  The path to an application.dll file to execute.
//
//C:\Users\simon>dotnet --help
//  .NET Command Line Tools (2.0.0)
//  Usage: dotnet [runtime - options] [path - to - application]
//  Usage: dotnet [sdk - options] [command][arguments][command - options]
//
//  path-to-application:
//  The path to an application.dll file to execute.
//
//  SDK commands:
//  new     Initialize.NET projects.
//  restore Restore dependencies specified in the.NET project.
//  run     Compiles and immediately executes a .NET project.
//  build   Builds a.NET project.
//  publish Publishes a.NET project for deployment (including the runtime).
//  test    Runs unit tests using the test runner specified in the project.
//  pack    Creates a NuGet package.
//  migrate Migrates a project.json based project to a msbuild based project.
//  clean   Clean build output(s).
//  sln     Modify solution (SLN) files.
//  add     Add reference to the project.
//  remove  Remove reference from the project.
//  list    List reference in the project.
//  nuget   Provides additional NuGet commands.
//  msbuild Runs Microsoft Build Engine (MSBuild).
//  vstest  Runs Microsoft Test Execution Command Line Tool.
//
//
//  Common options:
//  -v|--verbosity Set the verbosity level of the command. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].
//  -h|--help Show help.
//
//  Run 'dotnet COMMAND --help' for more information on a command.
//
//  sdk-options:
//  --version Display .NET Core SDK version.
//  --info Display .NET Core information.
//  -d|--diagnostics Enable diagnostic output.
//
//  runtime-options:
//  --additionalprobingpath<path> Path containing probing policy and assemblies to probe for.
//  --fx-version<version> Version of the installed Shared Framework to use to run the application.
//  --roll-forward-on-no-candidate-fx Roll forward on no candidate shared framework is enabled.
//  --additional-deps<path> Path to additonal deps.json file.
// 
//  C:\Users\simon>dotnet new  --help
//  Usage: new [options]
//
//  Options:
//  -h, --help Displays help for this command.
//  -l, --list Lists templates containing the specified name.If no name is specified, lists all templates.
//  -n, --name The name for the output being created. If no name is specified, the name of the current directory is used.
//  -o, --output Location to place the generated output.
//  -i, --install Installs a source or a template pack.
//  -u, --uninstall Uninstalls a source or a template pack.
//  --type Filters templates based on available types.Predefined values are "project", "item" or "other".
//  --force Forces content to be generated even if it would change existing files.
//  -lang, --language Specifies the language of the template to create.
// 
//  Examples:
//  dotnet new mvc --auth Individual
//
//  dotnet new webapi
//
//  dotnet new --help
/// 
/// 
/// 
/// </summary>
namespace pns
{

    public class paramanager
    {

        #region class-properties
        private string _opening = "{'" + '"';
        private string _closing = "}'" + '"';
        // public string CRLF = System.Environment.NewLine;
        public string helpindent = "  ";
        public bool takefirstvalueonly = true;
        private List<command> _commands = new List<command>();
        private List<lookup> _values = new List<lookup>();
        public void ClearAll()
        {
            this._commands.Clear();
            this._values.Clear();
        }
        #endregion class-properties

        #region parsing
        public bool Parse(string rawtext)
        {
            // assign the parts into sections, decorate these, and assign it to the _values list
            List<string> simple = this.GetSimpleParts(rawtext, this._opening, this._closing);
            List<lookup> parameterparts = this.GetParameterParts(simple);
            this._values.AddRange(parameterparts);
            return true;
        }
        public string GetParts()
        {
            StringBuilder sb = new StringBuilder();
            foreach (lookup thispart in this._values)
                sb.AppendLine(thispart.value);
            return sb.ToString();
        }
        public List<lookup> GetParameterParts(List<string> simpleparts)
        {

            // analyse this and extract each part into a simple list
            List<lookup> parameterparts = new List<lookup>();


            // set the 
            foreach (string thispart in simpleparts)
                parameterparts.Add(new lookup() { fullkey = "", parameterstyle = "Unknown", value = thispart });

            return parameterparts;
        }
        public List<string> GetSimpleParts(string rawtext, string opening, string closing)
        {
            // get the text for each part of the parameter string
            List<string> output = new List<string>();
            string currentpart = "";
            char currentclosure = ' ';
            foreach (char thischar in rawtext.ToCharArray())
            {
                if (thischar == ' ' && currentclosure == ' ')
                {
                    // this symbolises the end of the current item: plop it into the result
                    if(currentpart != "")
                    {
                        output.Add(currentpart);
                        currentpart = "";
                    }
                }
                else if(currentclosure == thischar)
                {
                    // end this enclosed section and plop it into the result
                    output.Add(currentpart);
                    currentpart = "";
                    currentclosure = ' ';
                }
                else if(opening.Contains(thischar))
                {
                    // this is the start of an enclosed part
                    currentpart = "";
                    currentclosure = closing[opening.IndexOf(thischar)];
                }
                else
                {
                    currentpart += thischar;
                }
            }
            return output;
        }
        public bool AnalyseParts()
        {
            return this.AnalyseParts(this._values);
        }
        public bool AnalyseParts(List<lookup> rawparts)
        {
            // step through each part and check it against the parameter rules
            // get all syn
            string currentparent = "";
            string currentstyle = "";
            foreach (lookup thispart in rawparts)
            {
                // check this in the synonyms
                command match = this._commands.FindAll(x => x.synonym.Equals(thispart.value) && x.parent.Equals(currentparent)).FirstOrDefault();
                if(match == null)
                {
                    // is a "data" item: set this in the type property of the parent
                    thispart.parameterstyle = "Data";
                    thispart.fullkey = currentparent;

                    // now put this into the "value" field of the parent
                    if (currentstyle == "Option" || currentstyle == "Command")
                    {
                        // find the parent and insert this value into the 
                        if(thispart.value != "")
                        {
                            lookup valuematch = this._values.FindAll(x => x.fullkey.Equals(thispart.fullkey)).FirstOrDefault();
                            if (valuematch.value == "")
                            {
                                // put this in
                                valuematch.value = thispart.value;
                            }
                            else if (this.takefirstvalueonly == false)
                            {
                                valuematch.value = valuematch.value + ' ' + thispart.value;
                            }
                        }
                    }
                }
                else
                {
                    // this is a match: find the key and retrieve this from 
                    command keymatch;
                    if (match.issynonym == true)
                    {
                        keymatch = this._commands.FindAll(x => x.key.Equals(match.key) && x.parent.Equals(currentparent)).FirstOrDefault();
                    }
                    else
                    {
                        keymatch = match;
                    }
                    if(keymatch != null)
                    {
                        // check the type: if a switch, we set it to true and do nothing else
                        if (keymatch.isswitch == true)
                        {
                            thispart.parameterstyle = "Switch";
                            thispart.fullkey = (currentparent + ' ' + keymatch.key).Trim();
                            thispart.value = "true";
                        }
                        else if (keymatch.isoption == true)
                        {
                            // the NEXT data item is the value: note that only the first of these is added
                            currentstyle = "Option";
                            currentparent = (currentparent + ' ' + keymatch.key).Trim();

                            thispart.parameterstyle = "Option";
                            thispart.fullkey = currentparent;
                            thispart.value = "";
                        }
                        else if (keymatch.iscommand == true)
                        {
                            // the NEXT data item is the value: note that only the first of these is added
                            currentstyle = "Command";
                            currentparent = (currentparent + ' ' + keymatch.key).Trim();

                            thispart.parameterstyle = "Command";
                            thispart.fullkey = currentparent;
                            thispart.value = "";
                        }

                    }
                }

      

            }

            return true;
        }

        #endregion parsing

        #region retrieving
        public string GetValue(string fullkey, string parameterstyle)
        {
            // return the appropriate value
            string output = "";
            lookup checkvalue = this._values.FindAll(x => x.fullkey.Equals(fullkey) && x.parameterstyle.Equals(parameterstyle)).FirstOrDefault();
            if (checkvalue != null)
                output = checkvalue.value;
            return output;
        }
        public string GetValue(string fullkey)
        {
            // return the FIRST appropriate value REGARDLESS of type
            string output = "";
            lookup checkvalue = this._values.FindAll(x => x.fullkey.Equals(fullkey)).FirstOrDefault();
            if (checkvalue != null)
                output = checkvalue.value;
            return output;
        }
        public bool GetSwitch(string fullkey)
        {
            // return true if this switch is present:
            // e.g. if the switch is "-o" and the "-o" parameter IS passed in
            return (this.GetValue(fullkey,"Switch") == "true");
        }
        public string GetOption(string fullkey)
        {
            return this.GetValue(fullkey, "Option");
        }
        public string GetCommand(string fullkey)
        {
            return this.GetValue(fullkey, "Command");
        }
        public string GetRoot(string fullkey)
        {
            return this.GetValue(fullkey, "Root");
        }
        #endregion retrieving

        #region parameter-creation
        private bool Add(string key, string parent, string shortdesc, string usage, string longdesc, string parameterstyle)
        {
            // the only distinction between commands and optionswitches is that commands can have sub-commands
            // find this key - if it does not exist, add it
            // if the parent does not exist, don't add it (unless is root level - blank)
            bool output = false;
            // the only distinction between commands and optionswitches is that commands can have sub-commands
            // find this key - if it does not exist, add it
            // if the parent does not exist, don't add it (unless is root level - blank)
            if (this.ParentExists(parent)
                && !this.KeyExists(key, parent)
                && (parameterstyle == "Command" || parameterstyle == "Switch" || parameterstyle == "Option" || parameterstyle == "Root")
                )
            {
                // if this is adding a switch, option, or command, do so
                // note that "" (root) always exists
                command newcommand = new command();
                newcommand.key = key;
                newcommand.synonym = key;
                newcommand.parent = parent;
                newcommand.shortdesc = shortdesc;
                newcommand.usage = usage;
                newcommand.longdesc = longdesc;
                newcommand.iscommand = (parameterstyle == "Command");
                newcommand.isoption = (parameterstyle == "Option");
                newcommand.isswitch = (parameterstyle == "Switch");
                newcommand.isroot = (parameterstyle == "Root");

                this._commands.Add(newcommand);
                output = true;
            }
            return output;
        }
        public bool AddRoot(string shortdesc, string usage, string longdesc)
        {
            return this.Add("", "", shortdesc, usage, longdesc, "Root");
        }
        public bool AddCommand(string key, string parent, string shortdesc, string usage, string longdesc)
        {
            return this.Add(key, parent, shortdesc, usage, longdesc, "Command");
        }
        public bool AddOption(string key, string parent, string shortdesc, string usage, string longdesc)
        {
            return this.Add(key, parent, shortdesc, usage, longdesc, "Option");
        }
        public bool AddSwitch(string key, string parent, string shortdesc, string usage, string longdesc)
        {
            return this.Add(key, parent, shortdesc, usage, longdesc, "Switch");
        }
        public bool AddSynonym(string key, string parent, string synonym)
        {
            bool output = false;
            // the only distinction between commands and optionswitches is that commands can have sub-commands
            // find this key - if it does not exist, add it
            // if the parent does not exist, don't add it (unless is root level - blank)
            if (this.ParentExists(parent) && this.KeyExists(key, parent))
            {
                // if this is adding a switch, option, or command, do so
                // note that "" (root) always exists
                command newcommand = new command();
                newcommand.key = key;
                newcommand.synonym = synonym;
                newcommand.parent = parent;
                newcommand.issynonym = true;

                this._commands.Add(newcommand);
                output = true;
            }
            return output;
        }
        public bool AddExample(string key, string parent, string example)
        {
            bool output = false;
            if (this.KeyExists(key, parent)) // && !this.SynonymExists(key, parent, synonym))
            {
                command addexample = _commands.FindAll(x => x.key.Equals(key) && x.parent.Equals(parent)).FirstOrDefault();
                addexample.examples.Add(example);
                output = true;
            }
            return output;
        }
        private bool ParentExists(string parent)
        {
            bool output = true;
            if (parent != "")
            {
                command checkparent = _commands.FindAll(x => x.key.Equals(parent)).FirstOrDefault();
                if (checkparent == null)
                    output = false;
            }
            return output;
        }
        private bool KeyExists(string key, string parent)
        {
            bool output = true;
            command checkkey = _commands.FindAll(x => x.key.Equals(key) && x.parent.Equals(parent)).FirstOrDefault();
            if (checkkey == null)
                output = false;
            return output;
        }
        private bool SynonymExists(string key, string parent, string synonym)
        {
            bool output = true;
            command checksynonym = _commands.FindAll(x => x.key.Equals(key) && x.parent.Equals(parent) && x.synonyms.Exists(y => y.Equals(synonym))).FirstOrDefault();
            if (checksynonym == null)
                output = false;
            return output;
        }
        public command FindCommand(string key, string parent)
        {
            // this gets the spscified command
            // note that the same key can exist in many places in the heirachy, but each key+parent combination is unique
            command output = _commands.FindAll(x => x.key.Equals(key) && x.parent.Equals(parent)).FirstOrDefault();
            return output;
        }
        public List<command> FindParents(string parent)
        {
            // this gets the spscified command
            // note that the same key can exist in many places in the heirachy, but each key+parent combination is unique
            List<command> output = _commands.FindAll(x => x.parent.Equals(parent));
            return output;
        }


        #endregion parameter-creation

        #region help-generation
        public string GetHelp()
        {
            return this.GetHelp("");
        }
        public string GetHelp(string fullkey)
        {
            // generate help for this
            //string cr = this.CRLF;
            StringBuilder sb = new StringBuilder();
            //command topcommand = _commands.FindAll(x => ((x.parent + ' ' + x.key).TrimEnd(new char[] { ' ' })).Equals(fullkey)).FirstOrDefault();


            IEnumerable<command> commandsearch =
                from x in _commands
                where ((x.parent + ' ' + x.key).Trim()) == fullkey
                select x;
            command topcommand = commandsearch.FirstOrDefault();

            // start by displaying long information
            if (topcommand.longdesc != "")
            {
                sb.AppendLine(topcommand.longdesc);
                sb.AppendLine();
            }


            if (topcommand.usage != "")
            {
                sb.AppendLine("Usage: " + topcommand.usage);
                sb.AppendLine();
            }

            // if there are any commands, put these into a section next
            List<command> allcommands = _commands.FindAll(x => x.parent.Equals(fullkey) && x.iscommand.Equals(true) && x.isroot.Equals(false));
            if (allcommands.Count > 0)
            {
                sb.AppendLine("Commands:");

                // we pad the commands to the width of the widest (including all synonyms
                List<string> commandsyntax = new List<string>();
                List<string> commandinfo = new List<string>();

                foreach (command thiscommand in allcommands)
                {
                    string thiscommandsyntax = thiscommand.key;
                    foreach (string thissynonym in thiscommand.synonyms)
                        thiscommandsyntax = thiscommandsyntax + '|' + thissynonym;
                    //                    thiscommandsyntax = thiscommandsyntax.TrimEnd("|".ToCharArray());
                    commandsyntax.Add(thiscommandsyntax);
                    commandinfo.Add(thiscommand.shortdesc);
                }
                int longestsyntax = commandsyntax.Max(x => x.Length);
                for (int thisline = 0; thisline < commandsyntax.Count; thisline++)
                    sb.AppendLine(this.helpindent + commandsyntax.ElementAt(thisline).PadRight(longestsyntax) + "  " + commandinfo.ElementAt(thisline));
                sb.AppendLine();
            }
            List<command> alloptions = _commands.FindAll(x => x.parent.Equals(fullkey) && (x.isoption.Equals(true) || x.isswitch.Equals(true)) && x.isroot.Equals(false));
            if (alloptions.Count > 0)
            {
                sb.AppendLine("Options:");

                // we pad the commands to the width of the widest (including all synonyms
                List<string> optionsyntax = new List<string>();
                List<string> optioninfo = new List<string>();

                foreach (command thisoption in alloptions)
                {
                    string thisoptionsyntax = thisoption.key;
                    foreach (string thissynonym in thisoption.synonyms)
                        thisoptionsyntax = thisoptionsyntax + '|' + thissynonym;
                    //                    thiscommandsyntax = thiscommandsyntax.TrimEnd("|".ToCharArray());
                    optionsyntax.Add(thisoptionsyntax);
                    optioninfo.Add(thisoption.shortdesc);
                }
                int longestsyntax = optionsyntax.Max(x => x.Length);
                for (int thisline = 0; thisline < optionsyntax.Count; thisline++)
                    sb.AppendLine(this.helpindent + optionsyntax.ElementAt(thisline).PadRight(longestsyntax) + "  " + optioninfo.ElementAt(thisline));
                sb.AppendLine();
            }

            if (topcommand.examples.Count > 0)
            {
                sb.AppendLine("Examples:");

                foreach (string thisexample in topcommand.examples)
                    sb.AppendLine(this.helpindent + thisexample);
                sb.AppendLine();
            }
            return sb.ToString();


            // the only distinction between commands and optionswitches is that commands can have sub-commands
            // find this key - if it does not exist, add it
            // if the parent does not exist, don't add it (unless is root level - blank)
            // bool output = false;
            //public bool SetParameters(string[] paraarray)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    foreach(string thispara in paraarray)
            //    {
            //        sb.Append(thispara);
            //        sb.Append(' ');
            //    }
            //    return this.SetParameters(sb.ToString().Trim());
            //}
            //public bool SetParameters(string paratext)
            //{
            //    // step through each character - note that once you have a command, you only check this and subcommands
            //    string rootcommand = "";

            //}

        }
        #endregion help-generation

    }

    public class command
    {
        public string key = "";
        public string synonym = "";
        public string parent = "";
        public string shortdesc = "";
        public string longdesc = "";
        public string usage = "";
        public List<string> synonyms = new List<string>();
        public List<string> examples = new List<string>();
        public bool iscommand = false;
        public bool isoption = false;
        public bool isswitch = false;
        public bool isroot = false;
        public bool issynonym = false;
    }

    public class lookup
    {
        public string fullkey = "";
        public string value = "";
        public string parameterstyle = "Unknown";
    }
}
