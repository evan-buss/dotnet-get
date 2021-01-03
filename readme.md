# Dotnet Get

> Inspired by Golang's `go get` this will install a dotnet tool by specifying a git repository URL.

# Usage

Requires `git` and `dotnet` to be available in your path.

## Install as a global tool.

`dotnet tool install DotnetGet --global`

## Install tool from repository.

`dotnet-get https://github.com/tool-repo-here`

# Notes

If your github repository has multiple projects. The tool will attempt to find all `Tool` projects by parsing the `*.csproj` file.
You will be prompted to select which tool to install.

The tool parses `*.csproj` files and looks for the following content to determine if the project
is a tool.

```xml
<PackAsTool>true</PackAsTool>
<ToolCommandName>tool-name</ToolCommandName>
<PackageOutputPath>./nupkg</PackageOutputPath>
```

Learn to create dotnet tools using the [official tutorial](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create).
