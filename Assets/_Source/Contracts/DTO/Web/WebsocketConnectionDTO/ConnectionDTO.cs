namespace _Source.Contracts.DTO.Web.WebsocketConnectionDTO
{
    public struct ConnectionDTO
    {
        public bool Connection { get; private set; }

        public ConnectionDTO(bool connection)
        {
            Connection = connection;
        }
    }
}

