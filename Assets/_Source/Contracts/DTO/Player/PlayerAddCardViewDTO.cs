namespace _Source.Contracts.DTO.Player
{
    public struct PlayerAddCardViewDTO
    {
        public string Name { get; private set; }

        public PlayerAddCardViewDTO(string name)
        {
            Name = name;
        }
    }
}