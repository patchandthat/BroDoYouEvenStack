﻿using Dota2GSI.Nodes;

namespace Dota2GSI.Nodes
{
    public enum DOTA_GameState
    {
        Undefined,
        DOTA_GAMERULES_STATE_DISCONNECT,
        DOTA_GAMERULES_STATE_GAME_IN_PROGRESS,
        DOTA_GAMERULES_STATE_HERO_SELECTION,
        DOTA_GAMERULES_STATE_INIT,
        DOTA_GAMERULES_STATE_LAST,
        DOTA_GAMERULES_STATE_POST_GAME,
        DOTA_GAMERULES_STATE_PRE_GAME,
        DOTA_GAMERULES_STATE_STRATEGY_TIME,
        DOTA_GAMERULES_STATE_WAIT_FOR_PLAYERS_TO_LOAD,
        DOTA_GAMERULES_STATE_CUSTOM_GAME_SETUP
    }

    public enum PlayerTeam
    {
        Undefined,
        None,
        Dire,
        Radiant
    }
    
    public class Map : Node
    {
        public readonly string Name;
        public readonly int MatchID;
        public readonly int GameTime;
        public readonly int ClockTime;
        public readonly bool IsDaytime;
        public readonly bool IsNightstalker_Night;
        public readonly DOTA_GameState GameState;
        public readonly PlayerTeam Win_team;
        public readonly string CustomGameName;
        public readonly int Ward_Purchase_Cooldown;

        internal Map(string json_data) : base(json_data)
        {
            Name = GetString("name");
            MatchID = GetInt("matchid");
            GameTime = GetInt("game_time");
            ClockTime = GetInt("clock_time");
            IsDaytime = GetBool("daytime");
            IsNightstalker_Night = GetBool("nightstalker_night");
            GameState = GetEnum<DOTA_GameState>("game_state");
            Win_team = GetEnum<PlayerTeam>("win_team");
            CustomGameName = GetString("customgamename");
            Ward_Purchase_Cooldown = GetInt("ward_purchase_cooldown");
        }
    }
}
