using static System.Console;
using System.Diagnostics;
using System.Data;

namespace SPHWM02 {

public class Program {

    public static int Main(string[] args) {
        foreach (var arg in args) {
            if (arg == "--daemonize") {
                return Daemon.Run();
            }
        }

        Process thisProc = Process.GetCurrentProcess();
        if (thisProc.MainModule == null) {
            WriteLine("Could not find main module of this process.");
            return 1;
        }
        string thisExePath = thisProc.MainModule.FileName;

        WriteLine("Entering interactive mode.");
        Process daemonProc = new Process {
            StartInfo =
                new ProcessStartInfo {
                    FileName = thisExePath,
                    Arguments = "--daemonize",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                },
        };
        daemonProc.Start();
        daemonProc.BeginOutputReadLine();
        WriteLine("Daemon process started.");

        bool shouldTerminate = false;
        bool promptDrawn = false;
        string input;

        daemonProc.OutputDataReceived += (sender, e) => {
            if (e.Data != null) {
                if (promptDrawn) {
                    Write("\x1b[2K\x1b[0G");
                }
                WriteLine($"Daemon: {e.Data}");
                if (promptDrawn)
                    Write("> ");
            }
        };
        while (!shouldTerminate) {
            Write("> ");
            promptDrawn = true;
            input = ReadLine() ?? "";
            promptDrawn = false;
            switch (input) {
            case "kill":
                WriteLine("Forcefully killing the daemon...");
                daemonProc.Kill();
                shouldTerminate = true;
                break;
            case "exit":
                if (!daemonProc.HasExited) {
                    daemonProc.StandardInput.WriteLine("terminate");
                }
                shouldTerminate = true;
                break;
            default:
                daemonProc.StandardInput.WriteLine(input);
                break;
            }
        }
        daemonProc.WaitForExit();
        WriteLine("Exiting...");
        return daemonProc.ExitCode;
    }
}

public static class Daemon {
    public static bool shouldTerminate = false;

    public static int Run() {
        string input, command;
        string[] args = {};
        string result;
        while (!shouldTerminate) {
            input = ReadLine() ?? "";
            var parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            command = parts.Length > 0 ? parts[0] : string.Empty;
            if (parts.Length > 1) {
                args = parts.Skip(1).Take(parts.Length).ToArray();
            }
            switch (command) {
            case "terminate":
                WriteLine("Terminating");
                shouldTerminate = true;
                break;
            case "calc":
                result = Calculate(args);
                WriteLine(result);
                break;
            case "wc":
                result = WordCount(args);
                WriteLine(result);
                break;
            default:
                WriteLine($"Unknown command: {command}");
                break;
            }
        }
        return 0;
    }

    public static string Calculate(string[] args) {
        if (args.Length < 1) {
            return "Error: No arguments provided to Calculate.";
        }
        try {
            string expression = string.Join(' ', args);
            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), expression);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return (string)row["expression"];

        } catch (Exception ex) {
            return $"Error: {ex.Message}";
        }
    }

    public static string WordCount(string[] args) {
        if (args.Length < 2) {
            return "Error: Not enough arguments. Expected 2, Path and Word.";
        }
        string path = args[0];
        string word = args[1];
        if (!File.Exists(path)) {
            return $"Error: The file at path '{path}' does not exist.";
        }
        try {
            string text = File.ReadAllText(path);
            string[] words =
                text.Split(new char[] { ' ', '\n', '\r', '\t', '.', ',', ';', '!', '?' },
                           StringSplitOptions.RemoveEmptyEntries);
            int count = 0;
            foreach (var w in words) {
                if (string.Equals(w, word, StringComparison.OrdinalIgnoreCase)) {
                    count++;
                }
            }
            return count.ToString();
        } catch (Exception ex) {
            return $"Error:  {ex.Message}";
        }
    }
}

} //! SPHMW02
