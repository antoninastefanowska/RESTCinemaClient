using CinemaClient.Client.Model;

namespace CinemaClient
{
    public class ReservationItem
    {
        public Reservation Reservation { get; set; }
        public Showing Showing { get; set; }
        public ReservationItem Self
        {
            get { return this; }
        }

        public ReservationItem(Reservation reservation, Showing showing)
        {
            Reservation = reservation;
            Showing = showing;
        }
    }
}
