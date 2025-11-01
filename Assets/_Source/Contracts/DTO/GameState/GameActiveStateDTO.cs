namespace _Source.Contracts.DTO.GameStage
{
    public struct GameActiveStateDTO
    {
        public bool GameActive { get; private set; }

        public GameActiveStateDTO(bool gameActive)
        {
            GameActive = gameActive;
        }
    }
}