using Grimtol.API.Models;
using System;
using System.Collections.Generic;

namespace Grimtol.API
{
    public class GameApi
    {

        Dictionary<string, IGameState> localGames = new Dictionary<string, IGameState>();

        public IGameState ProcessInput(string userInput, IGameState gameState)
        {
            if(userInput == null) { return gameState; }
            if (!ValidateGameState(gameState))
            { return null; }

            //Use stored state for processing commands
            var storedState = localGames[gameState.PlayerName];
            //extract userinput into commands and options
            userInput = userInput.ToLower();
            string command = userInput;
            string options = "";

            if (userInput.IndexOf(' ') > 0) {
                command = userInput.Substring(0, userInput.IndexOf(' ')).Trim();
                options = userInput.Substring(userInput.IndexOf(' ')).Trim();
            };


            IGameState updatedState = Game.ProcessCommand(command, options, storedState);
            localGames[storedState.PlayerName] = updatedState;
            return updatedState;


        }

        private bool ValidateGameState(IGameState gameState)
        {
            if (!localGames.ContainsKey(gameState.PlayerName))
            {
                return false;
            }
            return true;
        }

        public IGameState TryLoadGame(string playerName)
        {
            if (localGames.ContainsKey(playerName))
            {
                return Game.LoadGameState(localGames[playerName]);
            }
            var newGame = Game.Create(playerName);
            localGames.Add(playerName, newGame);
            return newGame;
        }
    }
}
