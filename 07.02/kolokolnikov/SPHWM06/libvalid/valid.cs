using System.Text.RegularExpressions;

namespace libvalid;

public static class valid {

    public static bool is_valid_full_name(string full_name) {
        if (string.IsNullOrEmpty(full_name))
            return false;
        return Regex.IsMatch(full_name, @"^[a-z\s-]+$",
                             RegexOptions.IgnoreCase);
    }

    public static bool is_valid_age(string age) {
        if (string.IsNullOrEmpty(age))
            return false;
        return Regex.IsMatch(age, @"^\d+$");
    }

    public static bool is_valid_phone(string phone) {
        if (string.IsNullOrEmpty(phone))
            return false;
        return Regex.IsMatch(
            phone, @"^\+?\d{1,3}\s*\(?\d{1,3}?\)?[\s-]?\d{3}[\s-]?\d{4}$");
    }

    public static bool is_valid_email(string email) {
        if (string.IsNullOrEmpty(email))
            return false;
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}
