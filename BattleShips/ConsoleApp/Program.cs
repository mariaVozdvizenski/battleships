using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DAL;
using Domain;
using Extensions;
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

            var menu = new Menu(MenuLevel.Level0);
            menu.AddNewMenuItem(new MenuItem("New game human vs human", "1", HumanVsHumanNewGame));
            menu.AddNewMenuItem(new MenuItem("New game human vs AI", "2", HumanVsAiNewGame));
            menu.AddNewMenuItem(new MenuItem("Load Game from Json", "3", LoadGameFromJson));
            menu.AddNewMenuItem(new MenuItem("Load Game from Db", "4", LoadGameFromDb));
            menu.RunMenu();
        }

        private static string HumanVsAiNewGame()
        {
            var menu = new Menu(MenuLevel.Level1);
            menu.AddNewMenuItem(new MenuItem("Save game", "S", null));

            (int height, _, _) = AskForUserInput("Please enter the board height", 20, 10, menu);
            (int width, _, _) = AskForUserInput("Please enter the board width", 20, 10, menu);

            Player player1 = new Player();
            Player player2 = new Player();

            BattleShips battleShips = new BattleShips(height, width, player1, player2);

            Console.WriteLine("Enter 'T' if the ships can touch. Enter any other symbol if not.");
            Console.Write(">");
            var touchRules = Console.ReadLine()?.Trim().ToUpper() ?? "E";

            if (touchRules != "T")
            {
                battleShips.ShipsCanTouch = false;
            }

            Console.Clear();
            Console.WriteLine("Player 1 ships");
            SetUpPlayerShips(battleShips, player1, menu);
            PlacePlayerShipsOnBoard(battleShips, player1, menu);

            Console.Clear();
            battleShips.AddDefaultShipsToPlayerShipList(player2);
            battleShips.PlaceShipsAutomatically(player2);
            battleShips.GameType = GameType.HumanVsAi;

            var userChoice = "";

            userChoice = HumanVsHumanMainGame(battleShips, menu);

            return userChoice!;
        }

        private static string LoadGameFromDb()
        {
            var userChoice = "";
            var menu = new Menu(MenuLevel.Level1);
            Console.Clear();
            using (var ctx = new AppDbContext())
            {
                var saveGames = ctx.BattleShipsSaves.ToList();
                if (DisplaySaveGames(menu, saveGames, out string quitToMenu)) return quitToMenu;
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
                        var battleShipsGame = new BattleShips(battleShipsSave);

                        menu.AddNewMenuItem(new MenuItem("Save game", "S", null));

                        userChoice = HumanVsHumanMainGame(battleShipsGame, menu);
                        break;
                    }

                    Console.WriteLine("This save doesn't exist.");
                } while (true);

                return userChoice;
            }
        }

        private static string LoadGameFromJson()
        {
            var userChoice = "";
            var menu = new Menu(MenuLevel.Level1);
            Console.Clear();
            var saveGames = SaveTool.LoadGamesFromFile();
            if (DisplaySaveGames(menu, saveGames, out string quitToMenu)) return quitToMenu;
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
                    var battleShipsGame = new BattleShips(battleShipsSave);

                    menu.AddNewMenuItem(new MenuItem("Save game", "S", null));

                    userChoice = HumanVsHumanMainGame(battleShipsGame, menu);
                    break;
                }

                Console.WriteLine("This save doesn't exist.");
            } while (true);

            return userChoice;
        }

        private static bool DisplaySaveGames<T>(Menu menu, ICollection<T> saveGames, out string loadGameFromJson)
            where T : BaseBattleShipsSave
        {
            menu.DisplayPredefinedMenuItems();
            Console.WriteLine("-----------");
            Console.WriteLine("Saved games");
            if (saveGames.Count == 0)
            {
                Console.WriteLine("No saves to load");
                WaitForUserInput("Press any key to return to main menu...");
                {
                    loadGameFromJson = "M";
                    return true;
                }
            }

            foreach (var saveGame in saveGames)
            {
                Console.WriteLine(saveGame.SaveName);
            }

            loadGameFromJson = "";
            return false;
        }

        static string HumanVsHumanNewGame()
        {
            var menu = new Menu(MenuLevel.Level1);
            menu.AddNewMenuItem(new MenuItem("Save game", "S", null));

            (int height, _, _) = AskForUserInput("Please enter the board height", 20, 10, menu);
            (int width, _, _) = AskForUserInput("Please enter the board width", 20, 10, menu);

            Player player1 = new Player();
            Player player2 = new Player();

            BattleShips battleShips = new BattleShips(height, width, player1, player2);

            Console.WriteLine("Enter 'T' if the ships can touch. Enter any other symbol if not.");
            Console.Write(">");
            var touchRules = Console.ReadLine()?.Trim().ToUpper() ?? "E";

            if (touchRules != "T")
            {
                battleShips.ShipsCanTouch = false;
            }

            Console.Clear();
            Console.WriteLine("Player 1 ships");
            SetUpPlayerShips(battleShips, player1, menu);
            PlacePlayerShipsOnBoard(battleShips, player1, menu);

            Console.Clear();
            Console.WriteLine("Player 2 ships");
            SetUpPlayerShips(battleShips, player2, menu);
            PlacePlayerShipsOnBoard(battleShips, player2, menu);

            var userChoice = "";

            userChoice = HumanVsHumanMainGame(battleShips, menu);

            return userChoice!;
        }

        private static void PlacePlayerShipsOnBoard(BattleShips battleShips, Player player, Menu menu)
        {
            string userShipPlacementChoice = "";
            do
            {
                Console.WriteLine("Press 'R' for random placement or press 'C' to place the ships yourself.");
                Console.Write(">");
                userShipPlacementChoice = Console.ReadLine()?.Trim().ToUpper() ?? "R";
            } while (userShipPlacementChoice != "R" && userShipPlacementChoice != "C");

            if (userShipPlacementChoice == "R")
            {
                battleShips.PlaceShipsAutomatically(player);
            }
            else
            {
                int nrOfShipsLeft = player.Ships.Count;

                foreach (var playerShip in player.Ships)
                {
                    BattleShipsUI.PrintBoard(battleShips, player);
                    Console.WriteLine($"Nr. of ships left to place: {nrOfShipsLeft}");
                    Console.WriteLine($"Ship width: {playerShip.Width}");
                    var shipOrientation = "";
                    do
                    {
                        Console.WriteLine("Ship orientation: Choose 'V' for vertical and 'H' for horizontal");
                        Console.Write(">");
                        shipOrientation = Console.ReadLine()?.Trim().ToUpper() ?? "V";
                    } while (shipOrientation != "V" && shipOrientation != "H");

                    Console.WriteLine($"Orientation: {shipOrientation}");
                    do
                    {
                        int startRow;
                        int startCol;

                        (startRow, _, _) = AskForUserInput("Choose the starting row coordinate for your ship",
                            battleShips.Height, 1, menu);
                        (startCol, _, _) = AskForUserInput("Choose the starting column coordinate for your ship",
                            battleShips.Width, 1, menu);

                        startRow--;
                        startCol--;

                        int endRow = startRow;
                        int endCol = startCol;

                        if (shipOrientation == "H")
                        {
                            for (var i = 1; i < playerShip.Width; i++)
                            {
                                endCol++;
                            }

                            playerShip.Orientation = ShipOrientation.Horizontal;
                        }
                        else
                        {
                            for (var i = 1; i < playerShip.Width; i++)
                            {
                                endRow++;
                            }

                            playerShip.Orientation = ShipOrientation.Horizontal;
                        }

                        if (endRow > battleShips.Height - 1 || endCol > battleShips.Width - 1)
                        {
                            Console.WriteLine("Ship ending row/column coordinate is out of bounds! Please try again");
                            continue;
                        }

                        List<Panel> affectedPanels = new List<Panel>();

                        if (!battleShips.ShipsCanTouch)
                        {
                            var orientation = shipOrientation == "H" ? 0 : 1;
                            battleShips.FindAffectedPanelsAroundTheShip(player, orientation, affectedPanels, startRow,
                                startCol, endRow, endCol);
                        }
                        else
                        {
                            affectedPanels.AddRange(player.GameBoard.Range(startRow, startCol, endRow, endCol));
                        }

                        var shipPlacementPanels = player.GameBoard.Range(startRow, startCol, endRow, endCol);

                        if (affectedPanels.Any(x => x.IsOccupied))
                        {
                            var message = "A ship has already been placed here!";
                            Console.WriteLine(!battleShips.ShipsCanTouch
                                ? $"{message} Also the ships cannot touch! Please try again"
                                : $"{message} Please try again");
                            continue;
                        }

                        playerShip.StartCol = startCol;
                        playerShip.StartRow = startRow;
                        playerShip.EndCol = endCol;
                        playerShip.EndRow = endRow;

                        foreach (var panel in shipPlacementPanels)
                        {
                            panel.PanelState = playerShip.PanelState;
                        }

                        nrOfShipsLeft--;
                        break;
                    } while (true);
                }
            }
        }

        private static void SetUpPlayerShips(BattleShips battleShips, Player player, Menu menu)
        {
            string userInput;
            do
            {
                Console.WriteLine("Enter 'D' for a default ship setup or 'C' to create custom ships.");
                Console.Write(">");
                userInput = Console.ReadLine()?.Trim().ToUpper() ?? "D";
            } while (userInput != "D" && userInput != "C");

            if (userInput == "D")
            {
                battleShips.AddDefaultShipsToPlayerShipList(player);
            }
            else
            {
                do
                {
                    Console.WriteLine($"({player.Ships.Count + 1}) Enter ship name: ");
                    Console.Write(">");
                    var shipName = Console.ReadLine()?.Trim().ToUpper() ?? "Default";
                    var (shipWidth, _, _) = AskForUserInput($"({player.Ships.Count + 1}) Enter ship width",
                        battleShips.Width >= battleShips.Height ? battleShips.Width : battleShips.Height, 1, menu);
                    battleShips.AddShipToPlayerShipList(player, new Ship() {Name = shipName, Width = shipWidth});
                } while (player.Ships.Count < 5);
            }
        }

        private static string HumanVsHumanMainGame(BattleShips battleShips, Menu menu)
        {
            int x;
            int y;
            string? userChoice;
            bool userSaved;

            Player currentPlayer;

            do
            {
                currentPlayer = battleShips.Player1Turn ? battleShips.Player1 : battleShips.Player2;

                if (battleShips.GameType == GameType.HumanVsHuman || battleShips.Player1Turn)
                {
                    BattleShipsUI.PrintBoard(battleShips, currentPlayer);
                }

                menu.DisplayCustomMenuItems();
                menu.DisplayPredefinedMenuItems();

                Console.WriteLine(battleShips.Player1Turn ? "Player 1's turn" : "Player 2's turn");

                if (battleShips.GameType == GameType.HumanVsHuman || battleShips.Player1Turn)
                {
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
                        BattleShipsUI.PrintBoard(battleShips, currentPlayer);
                    } while (userSaved);

                    if (userChoice != null)
                    {
                        break;
                    }

                    var shipHasBeenHit = battleShips.FireAShot(currentPlayer, x - 1, y - 1);
                    BattleShipsUI.PrintBoard(battleShips, currentPlayer);
                    DisplayShotResult(shipHasBeenHit);
                }
                else
                {
                    var shipHasBeenHit = battleShips.FireAiShot();
                    BattleShipsUI.PrintBoard(battleShips, currentPlayer);
                    DisplayShotResult(shipHasBeenHit);
                }

                if (battleShips.Player1.HasLost || battleShips.Player2.HasLost)
                {
                    var message = battleShips.Player1.HasLost
                        ? "Player 2 has won the game!"
                        : "Player 1 has won the game!";
                    Console.WriteLine(message);
                    WaitForUserInput("Press any key to quit the game");
                    userChoice = "M";
                    break;
                }

                var turnMessage = battleShips.Player1Turn
                    ? "Player2's turn, enter any key to continue..."
                    : "Player1's turn, enter any key to continue...";
                WaitForUserInput(turnMessage);
                battleShips.Player1Turn = !battleShips.Player1Turn;
            } while (true);

            return userChoice;
        }

        private static void DisplayShotResult(bool shipHasBeenShot)
        {
            if (shipHasBeenShot)
            {
                Console.WriteLine("A ship has been hit!");
                return;
            }

            Console.WriteLine("The shot missed!");
        }

        static string SaveGame(BattleShips game)
        {
            var saveName = AskUserForSaveGameName(1, 30);
            BattleShipsSaveJson battleShips = new BattleShipsSaveJson()
            {
                Height = game.Height,
                Width = game.Width,
                Player1 = game.Player1,
                Player1Turn = game.Player1Turn,
                Player2 = game.Player2,
                SaveName = saveName,
                GameType = game.GameType
            };
            
            SaveTool.SaveGameToFile(battleShips);

            using var ctx = new AppDbContext();
            var battleShipsSave = game.CreateBattleShipsSave(saveName);
            if (ctx.BattleShipsSaves.Any(e => e.SaveName == saveName))
            {
                var save = ctx.BattleShipsSaves.FirstOrDefault(e => e.SaveName == saveName);
                game.UpdateBattleShipsSave(save!);
                ctx.Update(save);
            }
            else
            {
                ctx.Add(battleShipsSave);
            }
            ctx.SaveChanges();

            return "";
        }

        private static string AskUserForSaveGameName(int min, int max)
        {
            using var ctx = new AppDbContext();
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

                if (SaveTool.SaveGameExists(userInput) || ctx.BattleShipsSaves.Any(e => e.SaveName == userInput))
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

        private static void WaitForUserInput(string prompt)
        {
            Console.WriteLine($"{prompt}");
            Console.Write(">");
            Console.ReadKey();
            Console.Clear();
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
                    }
                    else if (userInput < min)
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