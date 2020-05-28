using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaClient.Client.Model
{
    public class Showing : Entity
    {
        [JsonProperty("seatsRowNumber")]
        public int SeatsRowNumber { get; set; }

        [JsonProperty("seatsColumnNumber")]
        public int SeatsColumnNumber { get; set; }

        [JsonProperty("identifier")]
        public int Identifier { get; set; }

        [JsonProperty("filmId")]
        public int FilmID { get; set; }

        [JsonProperty("filmTitle")]
        public string FilmTitle { get; set; }

        [JsonProperty("dateEpoch")]
        public long DateEpoch { get; set; }

        [JsonProperty("takenSeats")]
        public List<Seat> TakenSeats { get; set; }
    }
}
