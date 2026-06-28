
namespace NetFrame.Models
{
    public class Db2Config
    {
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public bool AllowInsecureConnections { get; set; } = true;
    }
}
