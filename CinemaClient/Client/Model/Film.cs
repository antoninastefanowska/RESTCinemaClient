using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaClient.Client.Model
{
    public class Film : Entity
    {
        [JsonProperty("identifier")]
        public int Identifier { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("director")]
        public Person Director { get; set; }

        [JsonProperty("roles")]
        public List<Role> Roles { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        public byte[] Cover { get; set; }
    }
}
