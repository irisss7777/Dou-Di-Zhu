namespace _Source.Contracts.DTO.Lobby
{
    public struct AddPlayerToLobbyDTO
    {
        public string[] Name { get; private set; }
        public int CurrentPlayerCount { get; private set; }
        public int MaxPlayerCount { get; private set; }

        public AddPlayerToLobbyDTO(string[] name, int currentPlayerCount, int maxPlayerCount)
        {
            Name = name;
            CurrentPlayerCount = currentPlayerCount;
            MaxPlayerCount = maxPlayerCount;
        }
    }
}