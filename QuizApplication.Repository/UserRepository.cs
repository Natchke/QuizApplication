using Newtonsoft.Json;
using QuizApplication.Models;


namespace QuizApplication.Repository
{
    public class UserRepository
    {
        static string jsonFilePath = @"C:\Users\lenovo\source\repos\QuizApplication1\QuizApplication.Repository\AppData\ApplicationUserData.json";
        public static void Register()
        {
            Console.Clear();
            Console.WriteLine("    Register    ");

            Console.Write("Enter first name: ");
            string firstName = Console.ReadLine();

            Console.Write("Enter last name: ");
            string lastName = Console.ReadLine();

            Console.Write("Enter username: ");
            string userName = Console.ReadLine();

            List<User> users = LoadUsers();
            if (users.Any(u => u.UserName == userName))
            {
                Console.WriteLine("Username already exists!");
                Console.ReadLine();
                return;
            }

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            int id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
            User newUser = new User
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Password = password,
                HighScore = 0
            };

            users.Add(newUser);
            SaveUsers(users);

            Console.WriteLine("Registration successful!");
            Console.ReadLine();
        }

        public static void Login(List<User> users)
        {
            Console.Clear();
            Console.WriteLine("Login into your account");

            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            var user = users.FirstOrDefault(u => u.UserName == username && u.Password == password);

            if (user == null)
            {
                Console.WriteLine("There is no such account registered, please try again or register!");
                Console.ReadLine();
                return; 
            }

            Console.WriteLine($"Welcome back, {user.FirstName} {user.LastName}!");
            Console.ReadLine();
            UserMenu(user); 
        }
        public static void UserMenu(User user)
        {
            List<Quiz> quizzes = QuizRepository.LoadData(); 
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Welcome, {user.UserName}!");
                Console.WriteLine("1. Create Quiz");
                Console.WriteLine("2. Solve Quiz");
                Console.WriteLine("3. View My Quizzes");
                Console.WriteLine("4. Logout");
                Console.Write("Select an option: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        QuizRepository.CreateQuiz(quizzes, user);
                        break;
                    case "2":
                        QuizRepository.SolveQuiz(quizzes, user);
                        break;
                    case "3":
                        QuizRepository.ViewQuizzes(quizzes, user);
                        break;
                    case "4":
                        Console.WriteLine("You are logging out!");
                        return; 
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }


        public static void ShowTopPlayers(List<User> users)
        {
            Console.Clear();
            Console.WriteLine("   Top 10 Players   ");
            var topPlayers = users.OrderByDescending(u => u.HighScore).Take(10).ToList();
            if (!topPlayers.Any())
            {
                Console.WriteLine("No players to display yet.");
            }
            else
            {
                for (int i = 0; i < topPlayers.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {topPlayers[i].UserName} - {topPlayers[i].HighScore} points");
                }
            }

            Console.WriteLine("\nPress Enter to return to the main menu...");
            Console.ReadLine();
        }

        public static List<User> LoadUsers()
        {
            if (File.Exists(jsonFilePath))
            {
                string jsonData = File.ReadAllText(jsonFilePath);
                return JsonConvert.DeserializeObject<List<User>>(jsonData) ?? new List<User>();
            }
            else
            {
                return new List<User>();
            }
        }
        public static void SaveUserHighScore(User user)
        {
            string jsonFilePath = @"C:\Users\lenovo\source\repos\QuizApplication1\QuizApplication.Repository\AppData\ApplicationUserData.json";
            List<User> users = new List<User>();
            if (File.Exists(jsonFilePath))
            {
                string existingData = File.ReadAllText(jsonFilePath);
                users = JsonConvert.DeserializeObject<List<User>>(existingData) ?? new List<User>();
            }
            var existingUser = users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.HighScore = user.HighScore;
            }
            else
            {
                users.Add(user);
            }
            string jsonData = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }

        public static void SaveUsers(List<User> users)
        {
            string jsonData = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }
    }
}

