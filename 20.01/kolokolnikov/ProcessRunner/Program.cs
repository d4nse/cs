
using static System.Console;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ProcessRunner {
class Program {
    static void Main(string[] args) {
        WriteLine("Basic Shell");
        string input;
        bool shouldTerminate = false;
        while (!shouldTerminate) {
            Write("> ");
            input = ReadLine() ?? "";
            var pargs = input.Split(" ");
            if (pargs.Length > 0) {
                string command = pargs[0];
                string argstr = string.Join(" ", pargs, 1, pargs.Length - 1);
                switch (command) {
                case "exit":
                    shouldTerminate = true;
                    break;
                case "GetProcesses":
                    PrintProcesses();
                    break;
                case "GetProcessTree":
                    PrintProcessTree();
                    break;

                default:
                    try {
                        Process proc = new Process();
                        proc.StartInfo.FileName = command;
                        proc.StartInfo.Arguments = argstr;
                        proc.Start();
                        proc.WaitForExit();
                        WriteLine($"Process returned {proc.ExitCode}");
                    } catch (System.Exception er) {
                        WriteLine(er.Message);
                    }
                    break;
                }
            }
        }
    }
    static void PrintProcesses() {
        Process[] procs = Process.GetProcesses();
        foreach (var proc in procs) {
            WriteLine(" {0,-28}{1,-10}", proc.ProcessName, proc.Id);
        }
    }

    static void PrintProcessTree() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            WriteLine("This command utilises Unix specific features. Upgrade to Unix to use " +
                      "this command.");
            return;
        }
        var root = new ProcessNode { Name = "Root" };
        var procDict = new Dictionary<int, ProcessNode>();
        var cts = new CancellationTokenSource();
        var task = Task.Run(() => BuildProcessTree(root, procDict, cts.Token));
        bool is_tree_built = false;
        bool is_tree_failed = false;
        WriteLine("Building process tree...");
        task.ContinueWith(t => {
            if (t.IsCompletedSuccessfully) {
                WriteLine("Process tree has been built.");
                is_tree_built = true;
            } else {
                WriteLine("Failed to build the process tree.");
                is_tree_failed = true;
            }
        });
        while (!is_tree_failed) {
            if (is_tree_built) {
                WriteLine("\nType PID to view its info, 'list' to view processes, or 'exit'.");
                Write("proc > ");
                string input = ReadLine() ?? "";
                if (input == "exit") {
                    cts.Cancel();
                    break;
                } else if (input == "list") {
                    DisplayTree(root);
                } else {
                    if (int.TryParse(input, out int pid) &&
                        procDict.TryGetValue(pid, out ProcessNode? selectedProcess)) {
                        WriteLine(
                            $"Selected Process: {selectedProcess.Name} (PID: {selectedProcess.Pid})");
                        WriteLine($"Start Time: {selectedProcess.StartTime}");
                        WriteLine($"Memory Usage: {selectedProcess.MemoryUsage} KB");
                    } else {
                        WriteLine("Invalid PID.");
                    }
                }
            }
        }
        task.Wait();
    }

    static void BuildProcessTree(ProcessNode root, Dictionary<int, ProcessNode> procDict,
                                 CancellationToken cancelTok) {
        var procs = Process.GetProcesses();
        foreach (var proc in procs) {
            if (cancelTok.IsCancellationRequested)
                return;
            var node = new ProcessNode {
                Pid = proc.Id,
                Name = proc.ProcessName,
                StartTime = proc.StartTime,
                MemoryUsage = proc.WorkingSet64 / 1024,
            };
            procDict[proc.Id] = node;
            try {
                var parentId = GetParentProcessId(proc.Id);
                if (parentId != -1) {
                    var parentNode = procDict.GetValueOrDefault(parentId);
                    if (parentNode != null) {
                        parentNode.Children.Add(node);
                    } else {
                        root.Children.Add(node);
                    }
                } else {
                    root.Children.Add(node);
                }
            } catch (Exception) {
                root.Children.Add(node);
            }
        }
    }

    static int GetParentProcessId(int pid) {
        try {
            string path = $"/proc/{pid}/stat";
            if (File.Exists(path)) {
                var contents = File.ReadAllText(path);
                var parts = contents.Split(' ');
                return int.Parse(parts[3]);
            }
        } catch (Exception) {
        }
        return -1;
    }

    static void DisplayTree(ProcessNode node, string indent = "") {
        WriteLine($"{indent}- {node.Name} (PID: {node.Pid})");
        foreach (var child in node.Children) {
            DisplayTree(child, indent + "  ");
        }
    }

} //! Program

public class ProcessNode {
    public int Pid { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public long MemoryUsage { get; set; }
    public List<ProcessNode> Children { get; set; } = new List<ProcessNode>();
}

} //! ProcessProject
