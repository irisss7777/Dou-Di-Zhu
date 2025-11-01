namespace _Source.Contracts.DTO.Player
{
    public struct CurrentPlayerAddedDTO
    {
        public string Name { get; private set; }
        public string UserId { get; private set; }

        public CurrentPlayerAddedDTO(string name, string userId)
        {
            Name = name;
            UserId = userId;
        }
    }
}