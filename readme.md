﻿#  Dotnet Get

> Inspired by Golang's `go get` this will install a dotnet tool by specifying a git repository URL.

# Usage

1. `dotnet tool install dotnet-get --global`
2. `dotnet-get https://github.com/tool-repo-here`

# Notes

If your github repository has multiple projects. The tool will attempt to find all `Tool` projects by parsing the *.csproj file.
You will be prompted to select which tool to install.