using System;

namespace Web.New.Messages.Payloads.ClientServer
{
    [Serializable]
    public class ConnectPayload
    {
        public long lobby_id;
        public long user_id;
        public string room_name;
        public string user_name;
        public int balance;
        public int game_mode;
        public int currency_type;
        public int bet;
        public int max_bet;


        public ConnectPayload(long lobby_id, long user_id, string room_name, string user_name, int balance, int game_mode, int currency_type, int bet, int max_bet)
        {
            this.lobby_id = lobby_id;
            this.user_id = user_id;
            this.room_name = room_name;
            this.user_name = user_name;
            this.balance = balance;
            this.game_mode = game_mode;
            this.currency_type = currency_type;
            this.bet = bet;
            this.max_bet = max_bet;
        }
    }
}