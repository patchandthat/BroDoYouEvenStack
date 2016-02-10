using Dota2GSI.Nodes;

namespace Dota2GSI
{
    public class GameState : IGameState
    {
        private Newtonsoft.Json.Linq.JObject _ParsedData;
        private string json;

        private Auth auth;
        private Provider provider;
        private Map map;
        private Player player;
        private Hero hero;
        private Abilities abilities;
        private Items items;
        private GameState previously;
        private GameState added;

        /// <summary>
        /// Initialises a new GameState object from JSON Data
        /// </summary>
        /// <param name="json_data"></param>
        public GameState(string json_data)
        {
            if (json_data.Equals(""))
            {
                json_data = "{}";
            }

            json = json_data;
            _ParsedData = Newtonsoft.Json.Linq.JObject.Parse(json_data);
        }

        public Auth Auth
        {
            get { return auth ?? (auth = new Auth(GetNode("auth"))); }
        }

        public Provider Provider
        {
            get { return provider ?? (provider = new Provider(GetNode("provider"))); }
        }

        public Map Map
        {
            get { return map ?? (map = new Map(GetNode("map"))); }
        }

        public Player Player
        {
            get { return player ?? (player = new Player(GetNode("player"))); }
        }

        public Hero Hero
        {
            get { return hero ?? (hero = new Hero(GetNode("hero"))); }
        }

        public Abilities Abilities
        {
            get { return abilities ?? (abilities = new Abilities(GetNode("abilities"))); }
        }

        public Items Items
        {
            get { return items ?? (items = new Items(GetNode("items"))); }
        }

        public GameState Previously
        {
            get { return previously ?? (previously = new GameState(GetNode("previously"))); }
        }

        public GameState Added
        {
            get { return added ?? (added = new GameState(GetNode("added"))); }
        }

        private string GetNode(string name)
        {
            Newtonsoft.Json.Linq.JToken value;

            if (_ParsedData.TryGetValue(name, out value))
                return value.ToString();
            else
                return "";
        }

        public override string ToString()
        {
            return json;
        }
    }
}
