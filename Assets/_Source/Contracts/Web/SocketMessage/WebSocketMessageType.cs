using System;

namespace _Source.Contracts.Web.SocketMessage
{
    public enum WebSocketMessageType
    {
        USER_JOIN,
        NEW_USER_JOIN,
        USER_LEAVE,
        ALL_USER_INFO,
        GAME_MOVE,
        ADD_CARD,
        USER_PASS,
        CAN_USE_CARD,
        USE_CARD,
        USE_CARD_OTHER,
        PLAYER_PASS,
    }
}