using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaClient.Client.Model
{
    public abstract class Entity
    {
        [JsonProperty("links")]
        public List<Link> Links { get; set; }
    }
}
