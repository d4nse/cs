using static System.Console;
using libtrig;
using libnlp;
using libvalid;
using libfs;

namespace testapp {
public static class Program {
    public static void Main() {
        libtrigtest();
        libnlptest();
        libvalidtest();
        libfstest();
    }
    public static void libtrigtest() {
        WriteLine($"Area square (10): {Trig.AreaSquare(10)}");
        WriteLine($"Area rect (10, 20): {Trig.AreaRect(10, 20)}");
        WriteLine($"Area triangle (20, 15): {Trig.AreaTriangle(20, 15)}");
    }
    public static void libnlptest() {
        WriteLine($"Is 'aboba' palindrome?: {nlp.is_palin("aboba")}");
        WriteLine(
            $"Number of sentences in 'There's only one sentence.': {nlp.sentcount("There's only one sentence.")}");
        WriteLine($"Reverse of '69 is nice' is '{nlp.reverse("69 is nice")}'");
    }
    public static void libvalidtest() {
        WriteLine(
            $"Is 'John Doe' a valid full name?: {valid.is_valid_full_name("John Doe")}");
        WriteLine($"Is '25' a valid age?: {valid.is_valid_age("25")}");
        WriteLine(
            $"Is '+1 (555) 123-4567' a valid phone?: {valid.is_valid_phone("+1 (555) 123-4567")}");
        WriteLine(
            $"Is 'example@example.com' a valid email?: {valid.is_valid_email("example@example.com")}");
    }
    public static void libfstest() {
        try {
            string testFile = "test.txt";
            string reportFile = "search_report.txt";
            File.WriteAllText(
                testFile,
                "Hello World!\nThis is a test file.\nWord search is " +
                    "fun.\nAnother line without the keyword.\nEnd of file.");
            string searchWord = "test";
            fs.search_word_in_file(testFile, searchWord,
                                                      reportFile);
            WriteLine(
                $"Report generated for word search in file: {reportFile}");
            string testDir = "test_directory";
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "file1.txt"),
                              "This test file contains the word test.");
            File.WriteAllText(Path.Combine(testDir, "file2.txt"),
                              "This file does not have the keyword.");
            File.WriteAllText(Path.Combine(testDir, "file3.txt"),
                              "Another test file for word search.");
            string dirReportFile = "dir_search_report.txt";
            fs.search_word_in_directory(testDir, searchWord,
                                                           dirReportFile);
            WriteLine(
                $"Report generated for word search in directory: {dirReportFile}");
            File.Delete(testFile);
            File.Delete(reportFile);
            File.Delete(dirReportFile);
            Directory.Delete(testDir, true);
        } catch (Exception ex) {
            WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

}
