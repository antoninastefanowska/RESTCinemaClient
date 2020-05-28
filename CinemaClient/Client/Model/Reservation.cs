using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaClient.Client.Model
{
    public class Reservation : Entity
    {
        [JsonProperty("identifier")]
        public int Identifier { get; set; }

        [JsonProperty("showingId")]
        public int ShowingID { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("seats")]
        public List<Seat> Seats { get; set; }
    }
}
