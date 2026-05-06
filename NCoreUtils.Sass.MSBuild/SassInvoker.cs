using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace NCoreUtils;

public class SassInvoker
{
    public string ToolsPath { get; }

    public string ExecutablePath { get; }

    public int? Precision { get; }

    public IReadOnlyList<string> IncludePaths { get; }

    public string OutputStyle { get; }

    public bool GenerateSourceMap { get; }

    public SassInvoker(
        string toolsPath,
        int? precision = default,
        IReadOnlyList<string>? includePaths = default,
        string outputStyle = "compressed",
        bool generateSourceMap = false)
    {
        ToolsPath = toolsPath;
        ExecutablePath = RuntimeInformation.OSArchitecture switch
        {
            Architecture.X64 => RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? Path.Combine(ToolsPath, "linux-x64", "NCoreUtils.Sass.MSBuild.Runner")
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                    ? Path.Combine(ToolsPath, "osx-x64", "NCoreUtils.Sass.MSBuild.Runner")
                    : Path.Combine(ToolsPath, "win-x64", "NCoreUtils.Sass.MSBuild.Runner.exe"),
            Architecture.Arm64 => RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? Path.Combine(ToolsPath, "linux-arm64", "NCoreUtils.Sass.MSBuild.Runner")
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                    ? Path.Combine(ToolsPath, "osx-arm64", "NCoreUtils.Sass.MSBuild.Runner")
                    : Path.Combine(ToolsPath, "win-arm64", "NCoreUtils.Sass.MSBuild.Runner.exe"),
            var arch => throw new InvalidOperationException($"Unsupported architecture: {arch}")
        };
        Precision = precision;
        IncludePaths = includePaths ?? [];
        OutputStyle = outputStyle;
        GenerateSourceMap = generateSourceMap;
    }

    public void Invoke(string sourcePath, string targetPath)
    {
        var arguments = new List<string>(10)
        {
            $"--output-style {OutputStyle}"
        };
        if (IncludePaths.Count > 0)
        {
            foreach (var includePath in IncludePaths)
            {
                arguments.Add($"-i {includePath}");
            }
        }
        if (GenerateSourceMap)
        {
            arguments.Add("--source-maps");
        }
        arguments.Add($"-s \"{sourcePath}\"");
        arguments.Add($"-t \"{targetPath}\"");

        var info = new ProcessStartInfo
        {
            Arguments = string.Join(" ", arguments),
            CreateNoWindow = true,
            FileName = ExecutablePath,
            UseShellExecute = false
        };
        using var p = Process.Start(info);
        p.WaitForExit();
        if (p.ExitCode != 0)
        {
            throw new InvalidOperationException($"Executing {info.FileName} {info.Arguments} has failed with code {p.ExitCode}.");
        }
    }
}