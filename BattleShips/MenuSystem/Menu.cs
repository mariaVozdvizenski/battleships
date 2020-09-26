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
        private string[] PredefinedMenuItems { get; set; } = new[] {"X", "R", "M"};
        private readonly MenuLevel _menuLevel;

        public Menu(MenuLevel level)
        {
            _menuLevel = level;
            AddPredefinedMenuItemsToDict();
        }

        private void AddPredefinedMenuItemsToDict()
        {
            MenuItems.Add("X", new MenuItem("Exit", "X", null, true));
            MenuItems.Add("R", new MenuItem("Return to previous", "R", null, true));
            MenuItems.Add("M", new MenuItem("Return to main", "M", null, true));
        }
        
        public void AddNewMenuItem(MenuItem menuItem)
        {
            CheckMenuItemMinMaxLength(menuItem);
            MenuItems.Add(menuItem.UserChoice, menuItem);
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

                DisplayCustomMenuItems();
                DisplayPredefinedMenuItems();

                Console.Write(">");

                userChoice = Console.ReadLine()?.ToUpper().Trim() ?? "";

                if (!PredefinedMenuItems.Contains(userChoice))
                {
                    if (MenuItems.TryGetValue(userChoice, out var userMenuItem))
                    {
                        userChoice = userMenuItem.MethodToExecute!();
                    }
                    else
                    {
                        Console.WriteLine("I don't have this option!");
                    }
                }
                else
                {
                    DisplayErrorMessageBasedOnMenuLevel(userChoice);
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

        private void DisplayCustomMenuItems()
        {
            foreach (var menuItem in MenuItems)
            {
                if (!menuItem.Value.IsPredefined)
                {
                    Console.WriteLine(menuItem.Value);
                }
            }
        }

        private void DisplayErrorMessageBasedOnMenuLevel(string userChoice)
        {
            const string errorMessage = "Don't have this option!";

            switch (_menuLevel)
            {
                case MenuLevel.Level0:
                    if (userChoice != "X")
                    {
                        Console.WriteLine(errorMessage);
                    }
                    break;
                case MenuLevel.Level1:
                    if (userChoice != "X" && userChoice != "M")
                    {
                        Console.WriteLine(errorMessage);
                    }
                    break;
                case MenuLevel.Level2Plus:
                    if (userChoice != "X" && userChoice != "M" && userChoice != "R")
                    {
                        Console.WriteLine(errorMessage);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DisplayPredefinedMenuItems()
        {
            switch (_menuLevel)
            {
                case MenuLevel.Level0:
                    Console.WriteLine(MenuItems["X"]);
                    break;
                case MenuLevel.Level1:
                    Console.WriteLine(MenuItems["M"]);
                    Console.WriteLine(MenuItems["X"]);
                    break;
                case MenuLevel.Level2Plus:
                    Console.WriteLine(MenuItems["R"]);
                    Console.WriteLine(MenuItems["M"]);
                    Console.WriteLine(MenuItems["X"]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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