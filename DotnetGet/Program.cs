using System.CommandLine;
using System.CommandLine.Invocation;

var root = new RootCommand
{
    new Argument<string>("url",  "Git repository URL."),
};

root.Description = "Install dotnet tools without hosting on NuGet. Simply point to a git directory.";
root.Handler = CommandHandler.Create<string>(DotnetGet.DotnetGet.Start);
await root.InvokeAsync(args);