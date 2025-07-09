using System;
using System.IO;
using CommandLib;

namespace FileSystemCommands;

public class DirectorySizeCommand : ICommand
{
    private readonly string _path;

    public DirectorySizeCommand(string path)
    {
        _path = path;
    }

    public void Execute()
    {
        if (!Directory.Exists(_path))
        {
            Console.WriteLine($"Directory not found: {_path}");
            return;
        }

        long size = GetDirectorySize(new DirectoryInfo(_path));
        Console.WriteLine($"Total size of {_path}: {size} bytes");
    }

    private long GetDirectorySize(DirectoryInfo dir)
    {
        long size = 0;
        try
        {
            foreach (var file in dir.GetFiles())
                size += file.Length;

            foreach (var subDir in dir.GetDirectories())
                size += GetDirectorySize(subDir);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accessing directory {_path}: {ex.Message}");
        }

        return size;
    }
}
