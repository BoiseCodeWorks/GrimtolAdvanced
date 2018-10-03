using Grimtol.API;
using System;

namespace GrimtolAdvanced
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            var api = new GameApi();
            IGameState gameState;

            Console.WriteLine("Welcome Brave soul what is your name? ");
            var playerName = Console.ReadLine();

            gameState = api.TryLoadGame(playerName);
            string welcomeMessage = gameState.IsRestoredGame ? $"Welcome back {playerName}" : $"Welcome new adventurer I will record your history as the tale of {playerName}";

            Console.WriteLine(welcomeMessage);

            while (gameState.Active)
            {
                PrintGameState(gameState);
                gameState = api.ProcessInput(Console.ReadLine(), gameState);
            }

        }

        private static void PrintGameState(IGameState gameState)
        {
            string messageBox = "|  ";
            foreach(var m in gameState.Messages)
            {
                messageBox += $@"
|  {m}";
            }

            string inventoryItems = "";
            foreach (var item in gameState.Inventory)
            {
                inventoryItems += $" |- {item.Name} -| ";
            }

            Console.Clear();
            Console.WriteLine(@"
--------------------------------------------------------------------------------
  ____    _    ____ _____ _     _____    ____ ____  ___ __  __ _____ ___  _     
 / ___|  / \  / ___|_   _| |   | ____|  / ___|  _ \|_ _|  \/  |_   _/ _ \| |    
| |     / _ \ \___ \ | | | |   |  _|   | |  _| |_) || || |\/| | | || | | | |    
| |___ / ___ \ ___) || | | |___| |___  | |_| |  _ < | || |  | | | || |_| | |___ 
 \____/_/   \_|____/ |_| |_____|_____|  \____|_| \_|___|_|  |_| |_| \___/|_____|
");
            Console.WriteLine($@"
--------------------------------------------------------------------------------
|
|  Name: {gameState.PlayerName}
|  Health: {gameState.PlayerHealth}
|  CURRENT ROOM: {gameState.CurrentRoom.Name}
|  Inventory: {inventoryItems}
--------------------------------------------------------------------------------
|
|  Description: {gameState.CurrentRoom.Description}
{messageBox}
|
--------------------------------------------------------------------------------
", 0, Console.WindowWidth);
            Console.Write(@"
| What will you do?
| > ");



        }
    }
}
