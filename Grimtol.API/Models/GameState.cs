using System;
using System.Collections.Generic;

namespace Grimtol.API.Models
{
    [Serializable]
    internal class GameState : IGameState
    {
        public string PlayerName { get; }
        public string PlayerHealth { get; }
        public IEnumerable<IItem> Inventory { get; }
        public IRoom CurrentRoom { get; }
        public bool IsRestoredGame { get; }
        public bool Active { get; }
        public IEnumerable<string> Messages { get; }
        public IEnumerable<string> EventLog { get; }

        public GameState(Game game)
        {
            PlayerName = game.PlayerName;
            PlayerHealth = game.PlayerHealth.ToString();
            Inventory = game.Inventory;
            CurrentRoom = game.CurrentRoom;
            Active = game.Active;
            Messages = game.Messages;
            IsRestoredGame = game.Restored;
            EventLog = game.CompletedEvents;
        }
    }
}