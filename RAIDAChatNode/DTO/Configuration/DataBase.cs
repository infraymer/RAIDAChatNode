namespace RAIDAChatNode.DTO.Configuration
{
    public class DataBase
    {
        public string NameDB { get; set; }
        public string ConnectionString { get; set; }

        public DataBase()
        {
            NameDB = "sqlite";
            ConnectionString = "Filename=RAIDAChat.db";
        }
    }
}