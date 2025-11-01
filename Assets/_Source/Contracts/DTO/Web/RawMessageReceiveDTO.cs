namespace _Source.Contracts.DTO.Web
{
    public struct RawMessageReceiveDTO
    {
        public string Message { get; private set; }

        public RawMessageReceiveDTO(string message)
        {
            Message = message;
        }
    }
}