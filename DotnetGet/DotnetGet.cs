using CliWrap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotnetGet
{
    public static class DotnetGet
    {
        public record Tool(string Name, string Path, string Package);

        /// <summary>
        /// Start takes a git repository URL and attempts to install tools.
        /// </summary>
        /// <param name="url">Git repository URL.</param>
        /// <returns>Task that completes when installation is finished.</returns>
        public static async Task Start(string url)
        {
            string repoPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            await CloneRepository(url, repoPath);

            var tools = FindTools(repoPath);
            Console.WriteLine($"Discovered {tools.Count} {(tools.Count == 1 ? "tool" : "tools")}.");
            if (tools.Count == 0) return;

            for (int i = 0; i < tools.Count; i++)
            {
                Console.WriteLine($"\t{tools[i].Name} [{i + 1}]");
            }

            Console.Write("Tool to install: ");
            var ok = int.TryParse(Console.ReadLine(), out var index);
            Console.WriteLine();
            if (!ok || index < tools.Count || index > tools.Count)
            {
                Console.WriteLine("Invalid input.");
                return;
            }
            index--;

            await InstallTool(repoPath, tools[index]);
        }

        /// <summary>
        /// ClonesRepoistory clones a git repository to a specific path.
        /// </summary>
        /// <param name="url">URL when the repo is hosted.</param>
        /// <param name="path">Path where the repo will be cloned.</param>
        /// <returns>Task that completes when repo is finished being cloned.</returns>
        private static async Task CloneRepository(string url, string path)
        {
            try
            {
                Console.WriteLine("Cloning Repository");
                await Cli.Wrap("git").WithArguments($"clone {url} {path}").ExecuteAsync();
            }
            catch
            {
                Console.WriteLine("Error cloning repository.");
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// InstallTool installs builds and installs a tool from a csproj source.
        /// </summary>
        /// <param name="repoPath">Path to tool repository.</param>
        /// <param name="tool">Tool to build.</param>
        /// <returns>Task that completes when tool is installed.</returns>
        private static async Task InstallTool(string repoPath, Tool tool)
        {
            Console.WriteLine($"Building {tool.Name}.");
            await Cli.Wrap("dotnet")
                .WithWorkingDirectory(tool.Path)
                .WithArguments("pack")
                .ExecuteAsync();

            Console.WriteLine($"Installing {tool.Name}.");
            await Cli.Wrap("dotnet")
                .WithWorkingDirectory(tool.Path)
                .WithArguments($"tool install --global --add-source {tool.Package} {tool.Name}")
                .ExecuteAsync();

            Console.WriteLine("Cleaning up.");
            try
            {
                Directory.Delete(repoPath, true);
            }
            catch { }

            Console.WriteLine($"\n{tool.Name} has been installed. To uninstall:");
            Console.WriteLine($"\tdotnet tool uninstall {tool.Name} --global");
        }

        /// <summary>
        /// FindTools recursively searches a directory for *.csproj files for dotnet tools.
        /// Tool projects require
        /// </summary>
        /// <param name="repoPath">Path to cloned repository.</param>
        /// <returns>List of Tools that were found.</returns>
        private static List<Tool> FindTools(string repoPath)
        {
            var files = Directory.GetFiles(repoPath, "*.csproj", SearchOption.AllDirectories).ToList();

            var tools = new List<Tool>();

            foreach (var filePath in files)
            {
                var csproj = XElement.Load(filePath);

                var isTool = bool.Parse(csproj.Descendants("PackAsTool").FirstOrDefault()?.Value ?? "false");
                var toolName = csproj.Descendants("ToolCommandName").FirstOrDefault()?.Value;
                var package = csproj.Descendants("PackageOutputPath").FirstOrDefault()?.Value;

                if (isTool && toolName != null && package != null)
                {
                    tools.Add(new Tool(toolName, Path.GetDirectoryName(filePath) ?? "", package));
                }
            }

            return tools;
        }
    }
}