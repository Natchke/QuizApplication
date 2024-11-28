using System.Security.Cryptography.X509Certificates;
using QuizApplication.Models;
using QuizApplication.Repository;

namespace QuizApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
                List<User> users = UserRepository.LoadUsers(); 
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Welcome to Quiz App!");
                    Console.WriteLine("1. Register");
                    Console.WriteLine("2. Login");
                    Console.WriteLine("3. Show Top 10 Players");
                    Console.WriteLine("4. Exit");
                    Console.Write("Select an option: ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            UserRepository.Register();
                            break;
                        case "2":
                            UserRepository.Login(users);
                            break;
                        case "3":
                            UserRepository.ShowTopPlayers(users);
                            break;
                        case "4":
                            Console.WriteLine("Press enter to log out");
                            Console.ReadLine();
                            return; 
                        default:
                            Console.WriteLine("Invalid option. Try again.");
                            break;
                    }
                }
            }
     

    }
}

