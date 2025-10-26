internal class IterationChallenge2
{
    private static Dictionary<string, int> _grades = null!;
    private int examAssignments = 5;
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

        Dictionary<string, int[]> studentsScores = new Dictionary<string, int[]> {
            { "Sophia",[90, 86, 87, 98, 100, 94, 90]},
            { "Andrew",[92, 89, 81, 96, 90, 89] },
            { "Emma",[90, 85, 87, 98, 68, 89, 89, 89]},
            { "Logan",[90, 95, 87, 88, 96, 96]},
            { "Becky",[92, 91, 90, 91, 92, 92, 92]},
            { "Chris",[84, 86, 88, 90, 92, 94, 96, 98]},
            { "Eric",[80, 90, 100, 80, 90, 100, 80, 90]},
            { "Gregor",[91, 91, 91, 91, 91, 91, 91]}
        };

        double currentStudentExamScore = 0;
        double currentStudentExtraCreditScore = 0;
        double currentStudentGrade = 0;
        int gradedExtraCreditAssignments = 0;

        Console.WriteLine(String.Format("{0,-16}{1,-16}{2,-8}{3,-8}{4,-12}", ["Student", "Exam Score", "Overall", "Grade", "Extra Credit"]));
        foreach (KeyValuePair<string, int[]> studentScore in studentsScores)
        {
            gradedExtraCreditAssignments = studentScore.Value.Length - examAssignments;

            currentStudentExamScore = studentScore.Value.Take(examAssignments).Average();
            currentStudentExtraCreditScore = studentScore.Value.TakeLast(gradedExtraCreditAssignments).Average();
            currentStudentGrade = (studentScore.Value.Take(examAssignments).Sum() + ((double)studentScore.Value.TakeLast(gradedExtraCreditAssignments).Sum() / 10)) / examAssignments;

            Console.WriteLine(String.Format("{0,-16}{1,-16:F1}{2,-8:N}{3,-8}{4,-3}({5,-4:N} pts)", [
                studentScore.Key,
                currentStudentExamScore,
                currentStudentGrade,
                getStudentGradeLetterGrade(currentStudentGrade),
                currentStudentExtraCreditScore,
                ((studentScore.Value.TakeLast(gradedExtraCreditAssignments).Sum() / 10F) / (double)examAssignments)
                ]
            ));
        }

        Console.WriteLine("Press the Enter key to continue");
        Console.ReadLine();
    }

    private static string? getStudentGradeLetterGrade(double studentScore)
    {
        return _grades.OrderByDescending(grade => grade.Value).FirstOrDefault(grade => studentScore >= grade.Value).Key;
    }
}