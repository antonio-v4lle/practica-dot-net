internal class IterationChallengeI
{
    private static Dictionary<string, int> _grades = null!;
    public void run()
    {
        _grades = new Dictionary<string, int>()
        {
            {"A+", 97},
            {"A", 93},
            {"A-", 90},
            {"B+", 87},
            {"B", 83},
            {"B-", 80},
            {"C+", 77},
            {"C", 73},
            {"C-", 70},
            {"D+", 67},
            {"D", 63},
            {"D-", 60},
            {"F", 0}
        };

        Dictionary<string, int[]> studentScores = new Dictionary<string, int[]> {
            { "Sophia",[90, 86, 87, 98, 100, 94, 90]},
            { "Andrew",[92, 89, 81, 96, 90, 89] },
            { "Emma",[90, 85, 87, 98, 68, 89, 89, 89]},
            { "Logan",[90, 95, 87, 88, 96, 96]},
            { "Becky",[92, 91, 90, 91, 92, 92, 92]},
            { "Chris",[84, 86, 88, 90, 92, 94, 96, 98]},
            { "Eric",[80, 90, 100, 80, 90, 100, 80, 90]},
            { "Gregor",[91, 91, 91, 91, 91, 91, 91]}
        };

        Console.WriteLine(String.Format("{0,-10} {1,-5:N}", ["Student", "Grade"]));
        foreach (KeyValuePair<string, int[]> keyValuePair in studentScores)
        {
            Console.WriteLine(String.Format("{0,-10} {1,-8:N} {2,-2}", [keyValuePair.Key + ":", keyValuePair.Value.Average(), getStudentGrade(keyValuePair.Value.Average())]));
        }

        Console.WriteLine("Press the Enter key to continue");
        Console.ReadLine();
    }

    private static string? getStudentGrade(double studentScore)
    {
        return _grades.OrderByDescending(grade => grade.Value).FirstOrDefault(grade => studentScore >= grade.Value).Key;
    }
}