using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grimtol.API.Models
{
    internal enum Commands
    {
        go,
        take,
        use,
        quit
    }

    internal class Game
    {
        #region Properties
        internal Room CurrentRoom { get; private set; }
        internal bool Active { get; private set; }
        internal string PlayerName { get; private set; }
        internal int PlayerHealth { get; private set; } = 100;
        internal List<Item> Inventory { get; private set; } = new List<Item>();
        internal List<string> Messages { get; private set; } = new List<string>();
        internal Dictionary<string, Action> GameEvents { get; private set; } = new Dictionary<string, Action>();
        internal List<string> CompletedEvents { get; private set; } = new List<string>();
        internal bool Restored { get; private set; } = false;
        Dictionary<string, Room> _rooms;
        #endregion

        #region Public Methods
        #region Game Loaders
        static internal IGameState Create(string playerName)
        {
            return new Game(playerName).Setup();
        }

        static internal IGameState GetGameState(Game g)
        {
            return g.ToGameState();
        }

        internal static IGameState ProcessCommand(string command, string options, IGameState storedState)
        {
            return new Game(storedState).ProcessCommand(command, options);
        }

        internal static IGameState LoadGameState(IGameState gameState)
        {
            return new Game(gameState).ToGameState();
        }

        internal IGameState ToGameState()
        {
            return new GameState(this);
        }
        #endregion
        #endregion

        #region Private Methods

        #region GameLogic
        IGameState ProcessCommand(string command, string options)
        {
            if (!Enum.TryParse(command, out Commands valid))
            {
                Messages.Add($"[INVALID COMMAND] {command} {options}");
                return ToGameState();
            }
            switch (valid)
            {
                case Commands.go:
                    Go(options);
                    break;
                case Commands.take:
                    Take(options);
                    break;
                case Commands.use:
                    Use(options);
                    break;
                case Commands.quit:
                    Active = false;
                    break;
            }
            return ToGameState();
        }

        void Go(string direction)
        {
            if (CurrentRoom.Exits.ContainsKey(direction))
            {
                CurrentRoom = CurrentRoom.Exits[direction];
            }
            else
            {
                Messages.Add("It seems you can't move that way");
            }
        }

        void Take(string itemName)
        {
            if (string.IsNullOrEmpty(itemName)) { Messages.Add("Take what?"); return; }

            var item = CurrentRoom.TakeItem(itemName);
            if (item != null)
            {
                if (item.Takeable)
                {
                    Messages.Add($"Added {item.Name} to your inventory");
                    Inventory.Add(item);
                    CompletedEvents.Add($"{item.Name.ToUpper()}TAKEN");
                }
                else
                {
                    Messages.Add($"{item.NonTakeableMessage}");
                }
            }
            else
            {
                Messages.Add($"What are you talking about there is no {itemName}");
            }
        }

        void Use(string itemName)
        {
            Item item = Inventory.Find(i => i.Name == itemName);
            if (item != null)
            {
                Messages.Add($"You used {item.Name}");
                TriggerItemEvent(item);
            }
        }

        private void TriggerItemEvent(Item item)
        {
            if (CurrentRoom == _rooms["Locked"])
            {
                CompletedEvents.Add("KEYUSED");
                GameEvents["KEYUSED"].Invoke();
            }
        }
        #endregion

        #region Initialization
        IGameState Setup()
        {
            _rooms = new Dictionary<string, Room>();
            var entrance = new Room("Entrance", "The first room you encounter");
            var pickup = new Room("Pickup", "The pickup room");
            var locked = new Room("Locked", "A locked room");
            var exit = new Room("Exit", "This is the final room");

            _rooms.Add(entrance.Name, entrance);
            _rooms.Add(pickup.Name, pickup);
            _rooms.Add(locked.Name, locked);
            _rooms.Add(exit.Name, exit);


            entrance.Exits.Add("east", pickup);
            pickup.Exits.Add("west", entrance);
            pickup.Exits.Add("north", locked);
            locked.Exits.Add("south", pickup);
            pickup.AddItem(new Item() { Name = "key", Description = "a small key", Takeable = true });

            Active = true;
            CurrentRoom = entrance;

            RegisterGameEvents();
            HandleGameEvents();

            return ToGameState();
        }
        #endregion

        #region GameEventHandlers
        private void RegisterGameEvents()
        {
            GameEvents.Add("KEYTAKEN", RemoveKey);
            GameEvents.Add("KEYUSED", UnlockDoor);
        }
        private void HandleGameEvents()
        {
            CompletedEvents.ForEach(e =>
            {
                if (GameEvents.ContainsKey(e)) { GameEvents[e].Invoke(); } 
            });
        }

        private void RemoveKey()
        {
            Inventory.Add(_rooms["Pickup"].TakeItem("key"));
        }
        private void UnlockDoor()
        {
            _rooms["Locked"].Exits.Add("north", _rooms["Exit"]);
        }
        #endregion

        #region Constructors
        private Game(IGameState gameState)
        {
            PlayerName = gameState.PlayerName;
            Restored = true;
            foreach(var gameEvent in gameState.EventLog)
            {
                CompletedEvents.Add(gameEvent);
            }

            Setup();
            CurrentRoom = _rooms[gameState.CurrentRoom.Name];
        }

        private Game(string playerName)
        {
            PlayerName = playerName;
        }
        #endregion

        #endregion

    }
}
