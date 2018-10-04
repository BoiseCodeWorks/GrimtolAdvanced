using System;
using System.Collections.Generic;

namespace Grimtol.API.Models
{
    [Serializable]
    internal class Room : IRoom
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Dictionary<string, Room> Exits = new Dictionary<string, Room>();

        List<Item> _items = new List<Item>();

        public void AddItem(Item item)
        {
            _items.Add(item);
        }

        public Item TakeItem(string itemName)
        {
            var item = _items.Find(i => i.Name == itemName);
            if (item != null && item.Takeable) { _items.Remove(item); }
            return item;
        }

        public Room(string name, string description) {
            Name = name;
            Description = description;
        }
    }
}