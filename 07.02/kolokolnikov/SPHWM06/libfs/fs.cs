using System.Text;

namespace libfs;

public static class fs {

    public static void copy_file(string sourceFilePath, string destFilePath) {
        File.Copy(sourceFilePath, destFilePath, true);
    }

    public static void copy_directory(string sourceDir, string destDir) {
        Directory.CreateDirectory(destDir);
        foreach (var file in Directory.GetFiles(sourceDir)) {
            var fileName = Path.GetFileName(file);
            copy_file(file, Path.Combine(destDir, fileName));
        }
        foreach (var directory in Directory.GetDirectories(sourceDir)) {
            var directoryName = Path.GetFileName(directory);
            copy_directory(directory, Path.Combine(destDir, directoryName));
        }
    }

    public static void delete_file(string filePath) {
        if (File.Exists(filePath)) {
            File.Delete(filePath);
        }
    }

    public static void delete_files_by_names(string[] fileNames,
                                             string directoryPath) {
        foreach (var fileName in fileNames) {
            var filePath = Path.Combine(directoryPath, fileName);
            delete_file(filePath);
        }
    }

    public static void delete_files_by_mask(string mask, string directoryPath) {
        var files = Directory.GetFiles(directoryPath, mask);
        foreach (var file in files) {
            delete_file(file);
        }
    }

    public static void move_file(string sourceFilePath, string destFilePath) {
        File.Move(sourceFilePath, destFilePath);
    }

    public static void search_word_in_file(string filePath, string word,
                                           string reportPath) {
        if (!File.Exists(filePath)) {
            throw new FileNotFoundException(
                $"The file '{filePath}' does not exist.");
        }
        int lineCount = 0;
        int occurrenceCount = 0;
        StringBuilder report = new StringBuilder();
        using (var reader = new StreamReader(filePath)) {
            while (!reader.EndOfStream) {
                var line = reader.ReadLine() ?? "";
                lineCount++;
                if (line.Contains(word, StringComparison.OrdinalIgnoreCase)) {
                    occurrenceCount++;
                    report.AppendLine($"Line {lineCount}: {line}");
                }
            }
        }
        if (occurrenceCount > 0) {
            report.Insert(
                0,
                $"Word '{word}' found {occurrenceCount} time(s) in file '{filePath}'.\n");
        } else {
            report.AppendLine($"Word '{word}' not found in file '{filePath}'.");
        }
        File.WriteAllText(reportPath, report.ToString());
    }

    public static void search_word_in_directory(string directoryPath,
                                                string word,
                                                string reportPath) {
        if (!Directory.Exists(directoryPath)) {
            throw new DirectoryNotFoundException(
                $"The directory '{directoryPath}' does not exist.");
        }
        StringBuilder report = new StringBuilder();
        var files = Directory.GetFiles(directoryPath);
        foreach (var file in files) {
            int occurrenceCount = 0;
            int lineCount = 0;
            using (var reader = new StreamReader(file)) {
                while (!reader.EndOfStream) {
                    var line = reader.ReadLine() ?? "";
                    lineCount++;
                    if (line.Contains(word,
                                      StringComparison.OrdinalIgnoreCase)) {
                        occurrenceCount++;
                        report.AppendLine(
                            $"File: {file} | Line {lineCount}: {line}");
                    }
                }
            }
            if (occurrenceCount > 0) {
                report.Insert(
                    0,
                    $"Word '{word}' found {occurrenceCount} time(s) in file '{file}'.\n");
            }
        }
        if (report.Length == 0) {
            report.AppendLine(
                $"Word '{word}' not found in any files in directory '{directoryPath}'.");
        }
        File.WriteAllText(reportPath, report.ToString());
    }
}
