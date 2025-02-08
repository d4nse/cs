using System.Text.RegularExpressions;

namespace libnlp;

public class nlp {
    // im fucking tired of writing everything with capitals, fuck C#, fuck
    // microshit
    public static bool is_palin(string input) {
        if (string.IsNullOrEmpty(input))
            return false;
        string cleanedInput = Regex.Replace(input.ToLower(), @"\W", "");
        char[] charArray = cleanedInput.ToCharArray();
        Array.Reverse(charArray);
        return cleanedInput == new string(charArray);
    }

    public static int sentcount(string input) {
        if (string.IsNullOrEmpty(input))
            return 0;
        return Regex.Matches(input, @"[.!?]").Count;
    }

    public static string reverse(string input) {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        char[] charArray = input.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}
