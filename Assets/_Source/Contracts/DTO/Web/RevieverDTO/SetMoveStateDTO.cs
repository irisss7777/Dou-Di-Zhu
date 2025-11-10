using UnityEngine;

namespace _Source.Contracts.DTO.Web
{
    public struct SetMoveStateDTO
    {
        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public string LobbyId { get; private set; }
        public bool MoveState { get; private set; }
        public float Time { get; private set; }
        public float MaxTime { get; private set; }

        public SetMoveStateDTO(string userId, string userName, string lobbyId, bool moveState, float time, float maxTime)
        {
            UserId = userId;
            UserName = userName;
            LobbyId = lobbyId;
            MoveState = moveState;
            Time = time;
            MaxTime = maxTime;
        }
    }
}