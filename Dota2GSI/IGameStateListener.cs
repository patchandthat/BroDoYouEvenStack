namespace Dota2GSI
{
    public delegate void NewGameStateHandler(GameState gamestate);

    public interface IGameStateListener
    {
        GameState CurrentGameState { get; }

        /// <summary>
        /// Gets the port that is being listened
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Returns whether or not the listener is running
        /// </summary>
        bool Running { get; }

        /// <summary>
        ///  Event for handing a newly received game state
        /// </summary>
        event NewGameStateHandler NewGameState;

        /// <summary>
        /// Starts listening for GameState requests
        /// </summary>
        bool Start();

        /// <summary>
        /// Stops listening for GameState requests
        /// </summary>
        void Stop();
    }
}