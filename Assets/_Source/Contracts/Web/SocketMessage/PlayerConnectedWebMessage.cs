using System;

namespace _Source.Contracts.Web.SocketMessage
{
    public struct PlayerConnectedWebMessage
    {
        public string UserName { get; private set; }
        
        public PlayerConnectedWebMessage(string userName)
        {
            UserName = userName;
        }
    }
}