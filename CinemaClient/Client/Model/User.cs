using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaClient.Client.Model
{
    public class User
    {
        [JsonProperty("authorization")]
        public Authorization Authorization { get; set; }

        [JsonProperty("reservations")]
        public List<Reservation> Reservations { get; set; }
    }
}
