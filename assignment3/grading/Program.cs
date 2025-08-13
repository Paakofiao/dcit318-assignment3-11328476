using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70) return "B";
            if (Score >= 60) return "C";
            if (Score >= 50) return "D";
            return "F";
        }
    }

    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using (var reader = new StreamReader(inputFilePath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length != 3)
                        throw new MissingFieldException($"Invalid data format: {line}");

                    if (!int.TryParse(parts[0], out int id))
                        throw new FormatException($"Invalid ID format: {parts[0]}");

                    string name = parts[1].Trim();

                    if (!int.TryParse(parts[2], out int score))
                        throw new InvalidScoreFormatException($"Invalid score format for student {name}");

                    students.Add(new Student(id, name, score));
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var processor = new StudentResultProcessor();
            string inputFile = "students.txt";
            string outputFile = "report.txt";

            try
            {
                var students = processor.ReadStudentsFromFile(inputFile);
                processor.WriteReportToFile(students, outputFile);
                Console.WriteLine("Report generated successfully.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: The input file was not found.");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
