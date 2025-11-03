namespace _Source.Contracts.DTO.GameStage
{
    public struct LobbyInfoStateDTO
    {
        public bool LobbyActive { get; private set; }
        
        public LobbyInfoStateDTO(bool lobbyActive)
        {
            LobbyActive = lobbyActive;
        }
    }
}