using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QuizApplication.Models;

namespace QuizApplication.Repository
{
    public class QuizRepository
    {
        static string jsonQuizFilePath = @"C:\Users\lenovo\source\repos\QuizApplication1\QuizApplication.Repository\AppData\ApplicationQuizData.json";


        public static void CreateQuiz(List<Quiz> quizzes, User user)
        {
            Console.Clear();
            Console.WriteLine("   Create Quiz   ");
            Console.Write("Enter Quiz title: ");
            string title = Console.ReadLine();

            var quiz = new Quiz
            {
                QuizTitle = title,
                Creator = user.UserName,
                CreatorId = user.Id,
                QuizQuestions = new List<Question>()
            };

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"\nQuestion {i + 1}:");
                Console.Write("Enter question text: ");
                string questionText = Console.ReadLine();

                Console.Write("Enter correct answer: ");
                string correctAnswer = Console.ReadLine();

                var options = new List<string> { correctAnswer };
                while (options.Count < 4)
                {
                    Console.Write($"Enter incorrect answer {options.Count}: ");
                    options.Add(Console.ReadLine());
                }
                var random = new Random();
                quiz.QuizQuestions.Add(new Question
                {
                    QuestionText = questionText,
                    Options = options.OrderBy(_ => random.Next()).ToList(),
                    CorrectAnswer = correctAnswer
                });
            }
            //var existingData = JsonConvert.DeserializeObject<List<Quiz>>(File.ReadAllText(jsonQuizFilePath));
            //if (existingData != null)
            //{
            //    quizzes.AddRange(existingData);
            //}

            quizzes.Add(quiz);

            Console.WriteLine("\nQuiz created successfully!");
            SaveData(quizzes); 
            Console.ReadLine();
        }


        public static void ViewQuizzes(List<Quiz> quizzes, User user)
        {
            Console.Clear();
            Console.WriteLine("   My Quizzes   ");
            var myQuizzes = quizzes.Where(q => q.Creator == user.UserName).ToList();

            if (!myQuizzes.Any())
            {
                Console.WriteLine("You have not created any quizzes.");
                Console.ReadLine();
                return;
            }

            for (int i = 0; i < myQuizzes.Count; i++)
                Console.WriteLine($"{i + 1}. {myQuizzes[i].QuizTitle}");

            Console.Write("Select a quiz to edit or delete (or press Enter to go back): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > myQuizzes.Count)
                return;

            var quiz = myQuizzes[choice - 1];
            Console.WriteLine("\n1. Edit Quiz");
            Console.WriteLine("2. Delete Quiz");
            Console.Write("Select an option: ");
            switch (Console.ReadLine())
            {
                case "1":
                    EditQuiz(quiz, quizzes); 
                    break;
                case "2":
                    DeleteQuiz(quiz, quizzes); 
                    break;
                default:
                    Console.WriteLine("Invalid choice. Returning to menu.");
                    Console.ReadLine();
                    break;
            }
            SaveData(quizzes);
        }

        //esec ar momwons
        public static void EditQuiz(Quiz quiz, List<Quiz> quizzes)
        {
            Console.Clear();
            Console.WriteLine($"Editing Quiz: {quiz.QuizTitle}");

            Console.WriteLine("\n1. Edit Title");
            Console.WriteLine("2. Edit Questions");
            Console.Write("Select an option: ");
            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Enter new title: ");
                    quiz.QuizTitle = Console.ReadLine();
                    Console.WriteLine("Title updated successfully!");
                    break;

                case "2":
                    for (int i = 0; i < quiz.QuizQuestions.Count; i++)
                    {
                        Console.WriteLine($"\nQuestion {i + 1}: {quiz.QuizQuestions[i].QuestionText}");
                        Console.Write("Do you want to edit this question? (yes/no): ");
                        if (Console.ReadLine()?.ToLower() == "yes")
                        {
                            Console.Write("Enter new question text: ");
                            quiz.QuizQuestions[i].QuestionText = Console.ReadLine();

                            Console.Write("Enter correct answer: ");
                            string correctAnswer = Console.ReadLine();

                            var options = new List<string> { correctAnswer };
                            while (options.Count < 4)
                            {
                                Console.Write($"Enter incorrect answer {options.Count}: ");
                                options.Add(Console.ReadLine());
                            }

                            var random = new Random();
                            quiz.QuizQuestions[i].Options = options.OrderBy(_ => random.Next()).ToList();
                            quiz.QuizQuestions[i].CorrectAnswer = correctAnswer;

                            Console.WriteLine("Question updated successfully!");
                        }
                    }
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }

            //SaveData(quizzes); 
        }

        public static void DeleteQuiz(Quiz quiz, List<Quiz> quizzes)
        {
            quizzes.Remove(quiz);
            Console.WriteLine("Quiz deleted successfully!");
            //SaveData(quizzes); 
            Console.ReadLine();
        }
        public static void SolveQuiz(List<Quiz> quizzes, User user)
        {
            Console.Clear();
            Console.WriteLine("=== Solve Quiz ===");

            var availableQuizzes = quizzes.Where(q => q.CreatorId != user.Id).ToList();
            if (!availableQuizzes.Any())
            {
                Console.WriteLine("No quizzes available to solve.");
                Console.ReadLine();
                return;
            }

            var quiz = SelectQuiz(availableQuizzes);
            if (quiz == null) return;

            int score = SolveQuestions(quiz);
            Console.WriteLine($"You finished the quiz with a score of {score}!");
            if (score > user.HighScore)
            {
                Console.WriteLine("Congratulations! You set a new high score!");
                user.HighScore = score;
                UserRepository.SaveUserHighScore(user);
            }
            Console.ReadLine();
        }

        private static Quiz SelectQuiz(List<Quiz> quizzes)
        {
            for (int i = 0; i < quizzes.Count; i++)
                Console.WriteLine($"{i + 1}. {quizzes[i].QuizTitle} (By {quizzes[i].Creator})");

            Console.Write("Select a quiz: ");
            return int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= quizzes.Count
                ? quizzes[choice - 1]
                : null;
        }

        private static int SolveQuestions(Quiz quiz)
        {
            int score = 0;
            var timer = DateTime.Now;

            foreach (var question in quiz.QuizQuestions)
            {
                if ((DateTime.Now - timer).TotalMinutes > 2)
                {
                    Console.WriteLine("Time's up! You failed the quiz.");
                    Console.ReadLine();
                    break;
                }

                score = ProcessQuestion(question, score);
            }

            return score;
        }
        private static int ProcessQuestion(Question question, int score)
        {
            Console.Clear();
            Console.WriteLine(question.QuestionText);

            for (int i = 0; i < question.Options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {question.Options[i]}");
            }

            Console.Write("Select your answer: ");
            if (!int.TryParse(Console.ReadLine(), out int answer) || answer < 1 || answer > question.Options.Count)
            {
                Console.WriteLine("Invalid answer. Moving on...");
                score -= 20;
            }
            else if (question.Options[answer - 1] == question.CorrectAnswer)
            {
                Console.WriteLine("Correct!");
                score += 20;
            }
            else
            {
                Console.WriteLine("Incorrect!");
                score -= 20;
            }

            if (score < 0)
                score = 0;

            Console.ReadLine();
            return score;
        }
       public static List<Quiz> LoadData()
        {
            if (File.Exists(jsonQuizFilePath))
            {
                string jsonData = File.ReadAllText(jsonQuizFilePath);
                return JsonConvert.DeserializeObject<List<Quiz>>(jsonData) ?? new List<Quiz>();
            }
            else
            {
                return new List<Quiz>();
            }
        }
        public static void SaveData(List<Quiz> quizzes)
        {
            if (File.Exists(jsonQuizFilePath))
            {

                string jsonData = JsonConvert.SerializeObject(quizzes, Formatting.Indented);
                File.WriteAllText(jsonQuizFilePath, jsonData);
            }


        }
      

    }

}
