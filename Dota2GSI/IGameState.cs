using Dota2GSI.Nodes;

namespace Dota2GSI
{
    public interface IGameState
    {
        Auth Auth { get; }
        Provider Provider { get; }
        Map Map { get; }
        Player Player { get; }
        Hero Hero { get; }
        Abilities Abilities { get; }
        Items Items { get; }
        GameState Previously { get; }
        GameState Added { get; }
        string ToString();
    }
}