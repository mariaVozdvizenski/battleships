﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuSystem
{
    public enum MenuLevel
    {
        Level0,
        Level1,
        Level2Plus 
    }

    public enum PredefinedUserChoices
    {
        X,
        R,
        M
    }

    public class Menu
    {
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        private readonly MenuLevel _menuLevel;
        public Menu(MenuLevel level)
        {
            _menuLevel = level;
        }

        public void InitializeMenuItems(Action returnToMainMenuAction, Action? returnToPreviousMenuAction, Action exitAction)
        {
            switch (_menuLevel)
            {
                case MenuLevel.Level1:
                    MenuItems.Add(new MenuItem("Return to previous", "R", returnToMainMenuAction));
                    break;
                case MenuLevel.Level2Plus:
                    MenuItems.Add(new MenuItem("Return to previous", "R", returnToPreviousMenuAction!));
                    MenuItems.Add(new MenuItem("Return to main", "M", returnToMainMenuAction));
                    break;
                case MenuLevel.Level0:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            MenuItems.Add(new MenuItem("Exit", "X", exitAction));
        }

        public void AddNewMenuItem(MenuItem menuItem)
        {
            CheckMenuItemMinMaxLength(menuItem);
            
            if (MenuItems.All(m => m.UserChoice != menuItem.UserChoice) && 
                !Enum.IsDefined(typeof(PredefinedUserChoices), menuItem.UserChoice))
            {
                MenuItems.Add(menuItem);
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

        public void RunMenu()
        {
            var userChoice = "";
            do
            {
                Console.Write("");

                foreach (var menuItem in MenuItems)
                {
                    Console.WriteLine(menuItem);
                }
                
                Console.Write(">");
                
                userChoice = Console.ReadLine()?.ToUpper().Trim() ?? "";

                var userMenuItem = MenuItems.FirstOrDefault(t => t.UserChoice == userChoice);
                
                if (userMenuItem != null)
                {
                    if (userMenuItem.UserChoice == "X")
                    {
                        userMenuItem.MethodToExecute();
                        break;
                    }
                    userMenuItem.MethodToExecute();
                }
                else
                {
                    Console.WriteLine("I don't have this option!");
                }
                
            } while (userChoice != "X");
        }
    }
}
