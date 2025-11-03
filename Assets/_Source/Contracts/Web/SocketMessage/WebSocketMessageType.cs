using System;

namespace _Source.Contracts.Web.SocketMessage
{
    public enum WebSocketMessageType
    {
        CHAT_MESSAGE,
        USER_JOIN,
        NEW_USER_JOIN,
        USER_LEAVE,
        ALL_USER_INFO,
        GAME_STATE,
        GAME_MOVE,
        GAME_START,
        ERROR,
        ADD_CARD,
    }
}