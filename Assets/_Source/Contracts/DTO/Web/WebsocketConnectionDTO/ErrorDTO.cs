namespace _Source.Contracts.DTO.Web.WebsocketConnectionDTO
{
    public struct ErrorDTO
    {
        public string ErrorMessage { get; private set; }

        public ErrorDTO(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}