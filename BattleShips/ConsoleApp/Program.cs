using System;
using System.Linq;
using Domain;
using GameConsoleUI;
using GameEngine;
using MenuSystem;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;

            Console.WriteLine("==========> BATTLESHIPS <================");
            
            var menuB = new Menu(MenuLevel.Level2Plus);
            menuB.AddNewMenuItem(new MenuItem("Sub 2.", "1", DefaultMenuAction));
            
            var menuA = new Menu(MenuLevel.Level1);
            menuA.AddNewMenuItem(new MenuItem("Go to submenu 2", "1", menuB.RunMenu));
            
            var menu = new Menu(MenuLevel.Level0);
            menu.AddNewMenuItem(new MenuItem("Go to submenu 1", "S", menuA.RunMenu));
            menu.AddNewMenuItem(new MenuItem("New game human vs human", "1", HumanVsHumanNewGame));
            menu.AddNewMenuItem(new MenuItem("Load Game", "2", LoadGame));
            menu.RunMenu();
        }

        private static string LoadGame()
        {
            var userChoice = "";
            var menu = new Menu(MenuLevel.Level1);
            Console.Clear();
            var saveGames = SaveTool.LoadGamesFromFile();
            menu.DisplayPredefinedMenuItems();
            
            Console.WriteLine("-----------");
            Console.WriteLine("Saved games");
            foreach (var saveGame in saveGames)
            {
                Console.WriteLine(saveGame.SaveName);
            }
            do
            {
                Console.WriteLine("Please enter the name of a save");
                Console.Write(">");
                var userInput = Console.ReadLine();
                if (userInput!.ToUpper().Trim() == "X" || userInput.ToUpper().Trim() == "M")
                {
                    break;
                }
                Console.WriteLine(userInput);
                if (saveGames.Any(e => e.SaveName.Equals(userInput)))
                {
                    var battleShipsSave = saveGames.First(e => e.SaveName.Equals(userInput));
                    var battleShipsGame = new BattleShips(battleShipsSave.Height, battleShipsSave.Width,
                        battleShipsSave.Player1!,
                        battleShipsSave.Player2!, true) {Player1Turn = battleShipsSave.Player1Turn};
                    
                    menu.AddNewMenuItem(new MenuItem("Save game", "S", null));
                    
                    userChoice = HumanVsHumanMainGame(battleShipsGame, menu);
                    break;
                }
                Console.WriteLine("This save doesn't exist.");
            } while (true);
            return userChoice;
        }

        static string HumanVsHumanNewGame()
        {
            var menu = new Menu(MenuLevel.Level1);
            menu.AddNewMenuItem(new MenuItem("Save game", "S", null));
            
            (int height, _, _) = AskForUserInput("Please enter the board height", 20, 5, menu);
            (int width, _, _) = AskForUserInput("Please enter the board width", 20, 5, menu);
            
            Player player1 = new Player();
            Player player2 = new Player();
            
            BattleShips battleShips = new BattleShips(height, width, player1, player2);
            
            var userChoice = "";

            userChoice = HumanVsHumanMainGame(battleShips, menu);
            
            return userChoice!;
        }

        private static string HumanVsHumanMainGame(BattleShips battleShips, Menu menu)
        {
            int x;
            int y;
            string? userChoice;
            bool userSaved;

            do
            {
                BattleShipsUI.PrintBoard(battleShips);

                menu.DisplayCustomMenuItems();
                menu.DisplayPredefinedMenuItems();

                Console.WriteLine(battleShips.Player1Turn ? "Player 1's turn" : "Player 2's turn");

                (x, userChoice, userSaved) = AskForUserInput("Enter X coordinate", battleShips.Width, 1, menu);

                if (userChoice != null)
                {
                    break;
                }
                if (userSaved)
                {
                    SaveGame(battleShips);
                    continue;
                }
                do
                {
                    (y, userChoice, userSaved) = AskForUserInput("Enter Y coordinate", battleShips.Height, 1, menu);
                    if (!userSaved) continue;
                    SaveGame(battleShips);
                    BattleShipsUI.PrintBoard(battleShips);
                } while (userSaved);

                if (userChoice != null)
                {
                    break;
                }
                
                battleShips.FireAShot(x - 1, y - 1);
                BattleShipsUI.PrintBoard(battleShips);
                WaitForUserInput(battleShips.Player1Turn ? "Player2" : "Player1");
                battleShips.Player1Turn = !battleShips.Player1Turn;
                
            } while (true);

            return userChoice;
        }

        static string SaveGame(BattleShips game)
        {
            var saveName = AskUserForSaveGameName(1, 30);
            BattleShipsSave battleShips = new BattleShipsSave()
            {
                Height = game.Height,
                Width = game.Width,
                Player1 = game.Player1,
                Player1Turn = game.Player1Turn,
                Player2 = game.Player2,
                SaveName = saveName
            };
            SaveTool.SaveGameToFile(battleShips);
            return "";
        }

        private static string AskUserForSaveGameName(int min, int max)
        {
            var userInput = "";
            while (true)
            {
                Console.WriteLine("Please enter the name for a save");
                Console.Write(">");
                userInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput) || userInput.Length < min || userInput.Length > max)
                {
                    Console.WriteLine($"Save name has to be between {min} and {max} characters long.");
                    continue;
                }
                if (SaveTool.SaveGameExists(userInput))
                {
                    Console.WriteLine($"A save '{userInput}' already exists. Do you wish to overwrite it? (Y/N)");
                    var userAnswer = Console.ReadLine()?.Trim().ToUpper();
                    if (userAnswer != null && userAnswer.Equals("Y"))
                    { 
                        SaveTool.DeleteGameFromFile(userInput);
                       break; 
                    }
                    continue;
                }
                break;
            }
            return userInput;
        }
        

        private static void WaitForUserInput(string player)
        {
            Console.WriteLine($"{player}'s turn, enter any key to continue...");
            Console.Write(">");
            Console.ReadKey(); 
            Console.Clear();
        }
        
        static string DefaultMenuAction()
        {
            Console.WriteLine("Not implemented yet!");
            return "";
        }
        
        private static (int, string?, bool) AskForUserInput(string prompt, int max, int min, Menu menu)
        {
            do
            {
                Console.WriteLine(prompt);
                Console.Write(">");
                
                var consoleLine = Console.ReadLine()?.Trim().ToUpper();
                
                if (int.TryParse(consoleLine, out var userInput))
                {
                    if (userInput > max)
                    {
                        Console.WriteLine($"Cannot be bigger than {max}");
                    } else if (userInput < min)
                    {
                        Console.WriteLine($"Cannot be smaller than {min}");
                    }
                    else
                    {
                        return (userInput, null, false);
                    }
                }
                else if (menu.GetPredefinedItems().Contains(consoleLine))
                {
                    return (0, consoleLine, false);
                }
                else if (menu.MenuItemExists(consoleLine))
                {
                    return (0, null, true);
                }
                else 
                {
                    Console.WriteLine($"Input has to be a number between {min} and {max} or an exit value!");
                }

            } while (true);
        }
    }
}