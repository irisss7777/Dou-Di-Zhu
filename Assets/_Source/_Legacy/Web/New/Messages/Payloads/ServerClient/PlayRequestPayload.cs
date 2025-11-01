using System;

namespace Web.New.Messages.Payloads.ServerClient
{
    [Serializable]
    public class PlayRequestPayload
    {
        public long current_player; //кому предназначен запрос
        public long turn_player; //за кого должен походить
        public bool is_forced_turn;
        public int[] cards;
    }
}