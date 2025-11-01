namespace _Source.Contracts.DTO.Web
{
    public struct InputConnectionDTO
    {
        public bool Connection { get; private set; }

        public InputConnectionDTO(bool connection)
        {
            Connection = connection;
        }
    }
}