using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace RAIDAChatNode.DTO.Configuration
{
    public class DataBase
    {
        [Required]
        [RegularExpression(@"^(?i)\b(sqlite|mssql|mysql)\b$",
            ErrorMessage = "Configurations is not load: DataBase is not supported")]
        public string NameDB { get; set; }
        public string ConnectionString { get; set; }

        [Range(1, 25, ErrorMessage = "Configuration is not load: Count of reconnect to database must have range [1-25]")]
        public int CntConnect { get; set; }
        
        public DataBase()
        {
            NameDB = "sqlite";
            ConnectionString = "Filename=RAIDAChat.db";
            CntConnect = 10;
        }
    }
}