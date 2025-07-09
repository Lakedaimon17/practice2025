using System;
using System.IO;
using System.Linq;
using CommandLib;

namespace FileSystemCommands;

public class FindFilesCommand : ICommand
{
    private readonly string _directoryPath;
    private readonly string _searchPattern;

    public FindFilesCommand(string directoryPath, string searchPattern)
    {
        _directoryPath = directoryPath;
        _searchPattern = searchPattern;
    }

    public void Execute()
    {
        if (!Directory.Exists(_directoryPath))
        {
            Console.WriteLine($"Directory not found: {_directoryPath}");
            return;
        }

        var files = Directory.GetFiles(_directoryPath, _searchPattern, SearchOption.AllDirectories);
        Console.WriteLine($"Found {files.Length} files matching '{_searchPattern}':");
        foreach (var file in files)
        {
            Console.WriteLine(file);
        }
    }
}
