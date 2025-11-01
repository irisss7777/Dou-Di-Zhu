using System;
using System.Collections.Generic;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class GameStatePayload
    {
        public long lobby_id;
        public List<Player> players;
        public string phase; 
        public Landlord landlord;
        public int start_bet;
        public int max_bet;
        public int multiplier;
        public int[] kitty_cards;
        public long current_player_id;
    }

    [Serializable]
    public class Player
    {
        public long user_id;
        public string name = "undefined user";
        public int[] cards = new int[0];
        public int missed_turns = 0;
        public int[] last_move = new int[0];
        public int balance = 0;
        public bool is_connected = false;
        public int bet = 0;
        public string avatar_url = "";
    }

    [Serializable]
    public class Landlord
    {
        public bool selected;
        public long user_id;
    }
}
