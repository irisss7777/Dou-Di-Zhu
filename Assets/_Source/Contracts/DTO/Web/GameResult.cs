using System;

namespace _Source.Contracts.DTO.Web
{
    [Serializable]
    public class GameResult {
        public long user_id;
        public string role;
        public int result;
    }
}