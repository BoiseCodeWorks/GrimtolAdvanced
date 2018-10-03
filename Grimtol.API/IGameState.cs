using Grimtol.API.Models;
using System.Collections.Generic;

namespace Grimtol.API
{
    public interface IGameState
    {
        IRoom CurrentRoom { get; }
        string PlayerName { get; }
        string PlayerHealth { get; }
        bool IsRestoredGame { get; }
        bool Active { get; }
        IEnumerable<IItem> Inventory { get; }
        IEnumerable<string> Messages { get; }
        IEnumerable<string> EventLog { get; }
    }
}