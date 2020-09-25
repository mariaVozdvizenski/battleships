using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MenuSystem
{
    public enum MenuLevel
    {
        Level0,
        Level1,
        Level2Plus
    }

    public class Menu
    {
        private Dictionary<string, MenuItem> MenuItems { get; set; } = new Dictionary<string, MenuItem>();
        private readonly MenuLevel _menuLevel;
        private readonly string[] _predefinedActions = new[] {"M", "R", "X"};

        public Menu(MenuLevel level)
        {
            _menuLevel = level;
        }
        
        public void AddNewMenuItem(MenuItem menuItem)
        {
            CheckMenuItemMinMaxLength(menuItem);

            if (!_predefinedActions.Contains(menuItem.UserChoice))
            {
                MenuItems.Add(menuItem.UserChoice, menuItem);
            }
            else
            {
                throw new Exception($"A menu item with user choice '{menuItem.UserChoice}' already exists or is " +
                                    $"predefined.");
            }
        }

        private static void CheckMenuItemMinMaxLength(MenuItem menuItem)
        {
            if (string.IsNullOrWhiteSpace(menuItem.Label) || menuItem.Label.Length > 100)
            {
                throw new Exception($"Menu items' label has to be between 1 and 100 characters long.");
            }

            if (string.IsNullOrWhiteSpace(menuItem.UserChoice) || menuItem.UserChoice.Length > 5)
            {
                throw new Exception($"Menu items' user choice has to be between 1 and 5 characters long.");
            }
        }

        public string RunMenu()
        {
            var userChoice = "";
            do
            {
                Console.Write("");

                foreach (var menuItem in MenuItems)
                {
                    Console.WriteLine(menuItem.Value);
                }

                switch (_menuLevel)
                {
                    case MenuLevel.Level0:
                        Console.WriteLine("X) Exit");
                        break;
                    case MenuLevel.Level1:
                        Console.WriteLine("M) Return to main");
                        Console.WriteLine("X) Exit");
                        break;
                    case MenuLevel.Level2Plus:
                        Console.WriteLine("M) Return to main");
                        Console.WriteLine("R) Return to previous");
                        Console.WriteLine("X) Exit");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Console.Write(">");

                userChoice = Console.ReadLine()?.ToUpper().Trim() ?? "";

                if (!_predefinedActions.Contains(userChoice))
                {
                    if (MenuItems.TryGetValue(userChoice, out MenuItem userMenuItem))
                    {
                        userChoice = userMenuItem.MethodToExecute();
                    }
                    else
                    {
                        Console.WriteLine("I don't have this option!");
                    }
                }

                if (userChoice == "X")
                {
                    if (_menuLevel == MenuLevel.Level0)
                    {
                        SpinnerWithMessage("Closing down...");
                    }
                    break;
                }

                if (userChoice == "R" && _menuLevel == MenuLevel.Level2Plus)
                {
                    break;
                }

                if (userChoice == "M" && _menuLevel != MenuLevel.Level0)
                {
                    break;
                }
                
            } while (true);

            return userChoice;
        }
        
        private void SpinnerWithMessage(string message)
        {
            ConsoleSpinner spin = new ConsoleSpinner();
            Console.CursorVisible = false;
            Console.Write(message);
            
            Stopwatch s = new Stopwatch();
            
            s.Start();
            while (s.Elapsed < TimeSpan.FromSeconds(3)) 
            {
                spin.Turn();
            }

            Console.CursorVisible = true;
            s.Stop();
            Console.Clear();
        }
    }
}