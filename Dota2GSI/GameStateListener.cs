using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dota2GSI
{
    public class GameStateListener : IGameStateListener
    {
        private CancellationTokenSource _cancellation;

        private readonly HttpListener _listener;

        private GameState _currentGameState;

        public GameState CurrentGameState
        {
            get
            {
                return _currentGameState;
            }
            private set
            {
                _currentGameState = value;
                RaiseOnNewGameState();
            }
        }

        /// <summary>
        /// Gets the port that is being listened
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Returns whether or not the listener is running
        /// </summary>
        public bool Running => !_cancellation?.Token.IsCancellationRequested ?? false;

        /// <summary>
        ///  Event for handing a newly received game state
        /// </summary>
        public event NewGameStateHandler NewGameState = delegate { };

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public GameStateListener()
        {
        }

        /// <summary>
        /// A GameStateListener that listens for connections on http://localhost:port/
        /// </summary>
        /// <param name="port"></param>
        public GameStateListener(int port)
        {
            Port = port;
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:" + port + "/");
        }

        /// <summary>
        /// A GameStateListener that listens for connections to the specified URI
        /// </summary>
        /// <param name="uri">The URI to listen to</param>
        public GameStateListener(string uri)
        {
            if (!uri.EndsWith("/"))
                uri += "/";

            Regex uriPattern = new Regex("^https?:\\/\\/.+:([0-9]*)\\/$", RegexOptions.IgnoreCase);
            Match portMatch = uriPattern.Match(uri);

            if (!portMatch.Success)
                throw new ArgumentException("Not a valid URI: " + uri);

            Port = Convert.ToInt32(portMatch.Groups[1].Value);

            _listener = new HttpListener();
            _listener.Prefixes.Add(uri);
        }

        /// <summary>
        /// Starts listening for GameState requests
        /// </summary>
        public bool Start()
        {
            if (_cancellation == null)
            {
                _cancellation = new CancellationTokenSource();
                var token = _cancellation.Token;

                Task.Run(() => Run(token), token);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Stops listening for GameState requests
        /// </summary>
        public void Stop()
        {
            _cancellation.Cancel();

            _cancellation = null;
        }

        private async void Run(CancellationToken token)
        {
            _listener.Start();

            while (!token.IsCancellationRequested)
            {
                HttpListenerContext context = await _listener.GetContextAsync();

                var json = GetJsonFromGameContext(context);

                CurrentGameState = new GameState(json);
            }

            _listener.Stop();
        }

        private string GetJsonFromGameContext(HttpListenerContext context)
        {
            string json;
            var request = context.Request;

            using (Stream inputStream = request.InputStream)
            {
                using (StreamReader sr = new StreamReader(inputStream))
                    json = sr.ReadToEnd();
            }
            using (HttpListenerResponse response = context.Response)
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.StatusDescription = "OK";
                response.Close();
            }

            return json;
        }

        private void RaiseOnNewGameState()
        {
            var handler = NewGameState;

            handler?.Invoke(CurrentGameState);
        }
    }
}
