using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Grimtol.API.Models
{
    [Serializable]
    internal class GameState : IGameState
    {
        public string PlayerName { get; private set; }
        public string PlayerHealth { get; private set; }
        public IEnumerable<IItem> Inventory { get; private set; }
        public IRoom CurrentRoom { get; private set; }
        public bool IsRestoredGame { get; private set; }
        public bool Active { get; private set; }
        public IEnumerable<string> Messages { get; private set; }
        public IEnumerable<string> EventLog { get; private set; }


        private byte[] ToByteArray(IGameState obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        private GameState FromByteArray(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            GameState obj = (GameState)binForm.Deserialize(memStream);
            return obj;
        }


        public byte[] Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    writer.Write(PlayerName);
                    writer.Write(PlayerHealth);
                    writer.Write(CurrentRoom.Name);
                    var log = new List<string>();
                    foreach (var e in EventLog)
                    {
                        log.Add(e);
                    }
                    writer.Write(log.Count);
                    foreach(var e in EventLog)
                    {
                        writer.Write(e);
                    }
                }
                return m.ToArray();
            }
        }

        GameState Deserialize(byte[] data)
        {
            GameState result = new GameState();
            using (MemoryStream m = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(m))
                {
                    result.PlayerName = reader.ReadString();
                    result.PlayerHealth = reader.ReadString();
                    result.CurrentRoom = new Room(reader.ReadString(), "");
                    var log = new List<string>();
                    int eventCount = reader.ReadInt32();

                    while (eventCount > 0) {
                        log.Add(reader.ReadString());
                        eventCount -= 1;
                    }

                    result.EventLog = log;
                }
            }
            return result;
        }

        public bool Save()
        {
            var filename = $"{PlayerName}.GAMEDATA";
            File.WriteAllBytes(filename, ToByteArray(this));
            return true;
        }
        public GameState Load(string playerName)
        {
            var filename = $"{playerName}.GAMEDATA";
            if (!File.Exists(filename)) { return null; };
            return FromByteArray(File.ReadAllBytes(filename));
        }

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

        public GameState()
        {
        }
    }
}