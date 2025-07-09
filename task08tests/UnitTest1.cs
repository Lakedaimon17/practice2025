using System;
using System.IO;
using System.Reflection;
using CommandLib;
using FileSystemCommands;
using Xunit;

namespace Task08.Tests
{
    public class FileSystemCommandsTests
    {
        [Fact]
        public void DirectorySizeCommand_ShouldCalculateSize()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
            File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

            var command = new DirectorySizeCommand(testDir);
            command.Execute();

            Directory.Delete(testDir, true);
        }

        [Fact]
        public void FindFilesCommand_ShouldFindMatchingFiles()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
            File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

            var command = new FindFilesCommand(testDir, "*.txt");
            command.Execute();

            Directory.Delete(testDir, true);
        }
        [Fact]
        public void DirectorySizeCommand_WithNonExistingDirectory_ShouldShowError()
        {
            var command = new DirectorySizeCommand("nonexistent_dir");
            var output = CaptureConsoleOutput(command.Execute);

            Assert.Contains("Directory not found:", output);
        }

        [Fact]
        public void FindFilesCommand_WithNonExistingDirectory_ShouldShowError()
        {
            var command = new FindFilesCommand("nonexistent_dir", "*.txt");
            var output = CaptureConsoleOutput(command.Execute);

            Assert.Contains("Directory not found:", output);
        }

        [Fact]
        public void DirectorySizeCommand_ShouldReturnCorrectSize()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "SizeTestDir");
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "small.txt"), "12345");

            var command = new DirectorySizeCommand(testDir);
            var output = CaptureConsoleOutput(command.Execute);

            Assert.Contains("Total size of", output);
            Assert.Contains(": 5 bytes", output);

            Directory.Delete(testDir, true);
        }

        [Fact]
        public void FindFilesCommand_ShouldFindAllMatchingFiles()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "FindTestDir");
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "a.txt"), "text");
            File.WriteAllText(Path.Combine(testDir, "b.txt"), "text");
            File.WriteAllText(Path.Combine(testDir, "c.doc"), "doc");

            var command = new FindFilesCommand(testDir, "*.txt");
            var output = CaptureConsoleOutput(command.Execute);

            Assert.Contains("Found 2 files matching '*.txt':", output);
            Assert.Contains("a.txt", output);
            Assert.Contains("b.txt", output);

            Directory.Delete(testDir, true);
        }
        private string CaptureConsoleOutput(Action action)
        {
            var originalOut = Console.Out;
            using var sw = new StringWriter();
            Console.SetOut(sw);
            action();
            Console.SetOut(originalOut);
            return sw.ToString();
        }
    }
}
