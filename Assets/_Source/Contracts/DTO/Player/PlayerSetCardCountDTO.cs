namespace _Source.Contracts.DTO.Player
{
    public struct PlayerSetCardCountDTO
    {
        public string UserName { get; private set; }
        public int CardCount { get; private set; }

        public PlayerSetCardCountDTO(int cardCount, string userName)
        {
            CardCount = cardCount;
            UserName = userName;
        }
    }
}