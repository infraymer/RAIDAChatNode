using System.ComponentModel.DataAnnotations;

namespace RAIDAChatNode.DTO.Configuration
{
    public class SSL
    {
        [Required(ErrorMessage = "Configurations is not load: SSL IP is null")]
        [RegularExpression(@"^\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.|$)){4}\b$", 
            ErrorMessage = "Configurations is not load: SSL IP is not valid")]
        public string IP { get; set; }
        public string SerialNumb { get; set; }
        public string PathFile { get; set; }
        public string Password { get; set; }
    }
}