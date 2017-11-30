# cspara
Parameter manager for C#

This is a simple C# class for parsing parameters (usually passed in to a console application).

The terminology I have used for types of parameters is:
Switch - a parameter that, if passed-in, is set to 'true'.  It ignores characters after this.
Option - a parameter that is followed by text (which forms the value).
Command - an Option parameter that can also have sub-parameters: 
e.g. the syntax "dotnet.exe new -i mvc --force" will have:
"dotnet.exe" as the root level (this is a blank key plus blank parent in the application)
"new" is a Command, and this has "-i" and "--force" as sub-Options.  Each of these has "new" as the parent key.
"mvc" is a Switch with the parent key "new -i"

This is sample syntax to setup a paremter class:

pns.paramanager paramgr = new pns.paramanager();
paramgr.AddRoot("Command line dotnet management", "dotnet [commandname] [options]", "An application to manage .NET Core applications");
paramgr.AddSwitch("-v", "", "Show version number", "dotnet -v", "Displays the version number for this build of dotnet");
paramgr.AddSynonym("-v", "", "--version");
paramgr.AddSwitch("-h", "", "Show help", "dotnet -h", "Displays help for this command");
paramgr.AddSynonym("-h", "", "--help");
paramgr.AddCommand("new", "", "Initialize.NET projects.", "dotnet new [applicationname] [options]", "Run this to create a new .NET Core application in the current folder.");
paramgr.AddExample("new", "", "dotnet new mvc --auth Individual");
paramgr.AddExample("new", "", "dotnet new webapi");
paramgr.AddExample("new", "", "dotnet new --help");
paramgr.AddSwitch("-h", "new", "Show help", "dotnet new -h", "Displays help for creating a dotnet project");
paramgr.AddSynonym("-h", "new", "--help");
paramgr.AddOption("-o", "new", "Set output location", "dotnet new -o [outputlocation]", "Location to place the generated outcome");
paramgr.AddSynonym("-o", "new", "--output");
paramgr.AddOption("-i", "new", "Select source or template pack", "dotnet new -i [console|mvc|classlib|web]", "Type of dotnet package to create");
paramgr.AddSynonym("-i", "new", "--install");
paramgr.AddSwitch("console", "new -i", "Console application", "dotnet new -i console", "Create a new dotnet console application");
paramgr.AddSwitch("mvc", "new -i", "Console application", "dotnet new -i mvc", "Create a new dotnet MVC application");
paramgr.AddSwitch("classlib", "new -i", "Classlibrary", "dotnet new -i classlib", "Create a new dotnet classlibrary");
paramgr.AddSwitch("web", "new -i", "Web application", "dotnet new -i web", "Create a new dotnet web page");
paramgr.AddCommand("restore", "", "Restore .NET projects.", "dotnet restore [applicationname] [options]", "Restore dependencies specified in the .NET project");
paramgr.AddSynonym("restore", "", "rest");


After the structure is established, the application can try to parse text passed-in.

paramgr.Parse(@"new {-i }some """"thing   here -j{my var} -h-k  --install {some thi}");
paramgr.AnalyseParts();
Console.Write(paramgr.GetPartsInfo());
Console.WriteLine();
Console.WriteLine(paramgr.GetPartInfo("new"));
Console.WriteLine(paramgr.GetPartInfo("new -i"));
Console.WriteLine(paramgr.GetPartInfo("new -h"));
Console.WriteLine(paramgr.GetPartInfo("--help"));
Console.WriteLine(paramgr.GetPartInfo("new --help"));
Console.WriteLine(paramgr.GetPartInfo("new -h"));



