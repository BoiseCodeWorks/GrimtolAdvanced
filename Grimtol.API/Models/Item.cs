using System;

namespace Grimtol.API.Models
{
    public interface IItem
    {
        string Name { get; }
        string Description { get; }

    }
    [Serializable]
    internal class Item : IItem
    {
        public string Name { get; internal set; }
        public string NonTakeableMessage { get; internal set; }
        public string Description { get; internal set; }
        public bool Takeable { get; internal set; }
    }
}