using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaClient.Client.Model
{
    public class EntityList<T>
    {
        [JsonProperty("list")]
        public List<T> List { get; set; }

        public EntityList() { }

        public EntityList(List<T> list)
        {
            List = list;
        }
    }
}
