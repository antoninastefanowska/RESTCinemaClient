using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaClient.Client.Model
{
    public class Authorization
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        private string Password { get; set; }

        public Authorization(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string CreateHeader()
        {
            string header = "Basic ";
            string authString = Username + ":" + Password;
            byte[] data = Encoding.UTF8.GetBytes(authString);
            string encoded = Convert.ToBase64String(data);
            header += encoded;
            return header;
        }
    }
}
