using System.CommandLine;
using System.CommandLine.Invocation;

var root = new RootCommand
{
    new Argument<string>("url",  "Git repository URL."),
};

root.Description = "Install global dotnet tools directly from a Git repository.";
root.Handler = CommandHandler.Create<string>(DotnetGet.DotnetGet.Start);
await root.InvokeAsync(args);