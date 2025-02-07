using static System.Console;
using System.Collections.Concurrent;

namespace ResumeParser {

public class Resume {
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public decimal SalaryExpectation { get; set; }
}

public static class Program {
    public static void Main() {
        var filePaths = Directory.GetFiles("resumes", "*.txt");
        var resumes = ParseResumes(filePaths);
        var (minExp, maxExp) = GetExperienceReports(resumes);
        var (minSalary, maxSalary) = GetSalaryReports(resumes);
        var cityGroups = GetCandidatesByCity(resumes);
        WriteLine("Most Experienced: " + maxExp.Name);
        WriteLine("Least Experienced: " + minExp.Name);
        WriteLine("\nSalary Expectations:");
        WriteLine(
            $"Highest: {maxSalary.Name} - {maxSalary.SalaryExpectation:C}");
        WriteLine(
            $"Lowest: {minSalary.Name} - {minSalary.SalaryExpectation:C}");

        WriteLine("\nCandidates from the same city:");
        foreach (var group in cityGroups) {
            WriteLine(
                $"{group.Key}: {string.Join(", ", group.Value.Select(r => r.Name))}");
        }
    }

    public static List<Resume> ParseResumes(IEnumerable<string> filePaths) {
        var resumes = new ConcurrentBag<Resume>();
        Parallel.ForEach(filePaths, filePath => {
            try {
                var resume = new Resume();
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines) {
                    var parts = line.Split(':', 2);
                    if (parts.Length != 2)
                        continue;
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();
                    switch (key) {
                    case "Name":
                        resume.Name = value;
                        break;
                    case "YearsOfExperience":
                        if (int.TryParse(value, out var years))
                            resume.YearsOfExperience = years;
                        break;
                    case "City":
                        resume.City = value;
                        break;
                    case "SalaryExpectation":
                        if (decimal.TryParse(value, out var salary))
                            resume.SalaryExpectation = salary;
                        break;
                    }
                }
                resumes.Add(resume);
            } catch {
                WriteLine($"Error reading resume: {filePath}");
            }
        });
        return resumes.ToList();
    }

    public static (Resume minExp, Resume maxExp)
        GetExperienceReports(List<Resume> resumes) {
        var minExp = resumes.AsParallel().Aggregate(
            (r1, r2) => r1.YearsOfExperience < r2.YearsOfExperience ? r1 : r2);
        var maxExp = resumes.AsParallel().Aggregate(
            (r1, r2) => r1.YearsOfExperience > r2.YearsOfExperience ? r1 : r2);
        return (minExp, maxExp);
    }

    public static (Resume minSalary, Resume maxSalary)
        GetSalaryReports(List<Resume> resumes) {
        var minSalary = resumes.AsParallel().Aggregate(
            (r1, r2) => r1.SalaryExpectation < r2.SalaryExpectation ? r1 : r2);
        var maxSalary = resumes.AsParallel().Aggregate(
            (r1, r2) => r1.SalaryExpectation > r2.SalaryExpectation ? r1 : r2);
        return (minSalary, maxSalary);
    }

    public static Dictionary<string, List<Resume>>
    GetCandidatesByCity(List<Resume> resumes) {
        return resumes.AsParallel()
            .GroupBy(r => r.City)
            .Where(g => g.Count() > 1)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}

}
