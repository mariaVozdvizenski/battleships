using System;
using System.Diagnostics;
using MenuSystem;
using Program;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("=========> BATTLESHIPS <================");
            
            var menuB = new Menu(MenuLevel.Level2Plus);
            menuB.AddNewMenuItem(new MenuItem("Sub 2.", "1", DefaultMenuAction));
            var menuA = new Menu(MenuLevel.Level1);
            menuA.AddNewMenuItem(new MenuItem("Go to submenu 2", "1", menuB.RunMenu));
            var menu = new Menu(MenuLevel.Level0);

            menu.AddNewMenuItem(new MenuItem("Go to submenu 1", "S", menuA.RunMenu));
            menu.AddNewMenuItem(new MenuItem("New game human vs human. Pointless.", "1", DefaultMenuAction));
            menu.AddNewMenuItem(new MenuItem("New game puny human vs mighty AI", "2", DefaultMenuAction));
            menu.AddNewMenuItem(new MenuItem("New game mighty AI vs superior AI", "3", DefaultMenuAction));
            
            menu.InitializeMenuItems(menu.RunMenu, null, ExitAction);
            menuA.InitializeMenuItems(menu.RunMenu, null, ExitAction);
            menuB.InitializeMenuItems(menu.RunMenu, menuA.RunMenu, ExitAction);
            
            menu.RunMenu();
        }
        
        static void DefaultMenuAction()
        {
            Console.WriteLine("Not implemented yet!");
        }

        static void ExitAction()
        {
            SpinnerWithMessage("Closing down...");
            Environment.Exit(0);
        }
        
        private static void SpinnerWithMessage(string message)
        {
            ConsoleSpinner spin = new ConsoleSpinner();
            Console.CursorVisible = false;
            Console.Write(message);
            
            Stopwatch s = new Stopwatch();
            
            s.Start();
            while (s.Elapsed < TimeSpan.FromSeconds(5)) 
            {
                spin.Turn();
            }

            Console.CursorVisible = true;
            s.Stop();
            Console.Clear();
        }
    }
}