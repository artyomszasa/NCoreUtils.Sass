using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace NCoreUtils;

public class SassTask : Task
{
    private readonly List<ITaskItem> _generatedFiles = new List<ITaskItem>();

    [Required]
    public ITaskItem[] SassSources { get; set; } = [];

    [Required]
    public string ToolsPath { get; set; } = default!;

    [Output]
    public ITaskItem[] GeneratedFiles => [.. _generatedFiles];

    public override bool Execute()
    {
        var errors = new List<(string Path, Exception Error)>();
        foreach (var item in SassSources)
        {
            if (string.IsNullOrEmpty(item.ItemSpec))
            {
                continue;
            }
            var rootFiles = Directory.EnumerateFiles(item.ItemSpec, "*.sass", SearchOption.AllDirectories)
                .Concat(Directory.EnumerateFiles(item.ItemSpec, "*.scss", SearchOption.AllDirectories))
                .Where(path => !Path.GetFileName(path).StartsWith("_"))
                .ToList();
            var includePaths = item.GetMetadata("Include") switch
            {
                null or "" => [],
                var includes => includes.Split([';'], StringSplitOptions.RemoveEmptyEntries)
            };
            var invoker = new SassInvoker(ToolsPath, includePaths: includePaths);
            foreach (var path in rootFiles)
            {
                var target = Path.ChangeExtension(path, ".css");
                Log.LogMessageFromText($"Processing {path} -> {target}.", MessageImportance.High);
                try
                {
                    invoker.Invoke(path, target);
                    _generatedFiles.Add(new TaskItem(target));
                }
                catch (Exception exn)
                {
                    errors.Add((path, exn));
                }
            }
        }
        foreach (var (path, error) in errors)
        {
            Log.LogErrorFromException(error, true, true, path);
        }
        return errors.Count == 0;
    }
}