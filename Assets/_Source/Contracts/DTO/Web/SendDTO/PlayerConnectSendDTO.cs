using System;
using _Source.Contracts.Web.SocketMessage;
using UnityEngine;

namespace _Source.Contracts.DTO.Web.SendDTO
{
    public struct PlayerConnectSendDTO
    {
        public WebSocketMessageType Type { get; private set; }
        public PlayerConnectedWebMessage Data { get; private set; }
        
        public PlayerConnectSendDTO(PlayerConnectedWebMessage data, WebSocketMessageType type)
        {
            Type = type;
            Data = data;
        }
    }
}