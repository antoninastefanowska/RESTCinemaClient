using CinemaClient.Client;
using CinemaClient.Client.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CinemaClient
{
    public partial class MainWindow : Window
    {
        public Authorization Authorization { get; set; }
        public CinemaServiceClient Service { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadShowingsData();
        }

        private void StartLoading()
        {
            ProgressBar.Visibility = Visibility.Visible;
        }

        private void StopLoading()
        {
            ProgressBar.Visibility = Visibility.Collapsed;
        }

        private void ShowShowingButton_Click(object sender, RoutedEventArgs e)
        {
            Showing showing = ((FrameworkElement)sender).DataContext as Showing;
            FilmWindow filmWindow = new FilmWindow();
            filmWindow.Owner = this;
            filmWindow.Showing = showing;
            filmWindow.Authorization = Authorization;
            filmWindow.Service = Service;
            filmWindow.Show();
        }

        private void ShowReservationButton_Click(object sender, RoutedEventArgs e)
        {
            ReservationItem item = ((FrameworkElement)sender).DataContext as ReservationItem;
            ReservationWindow reservationWindow = new ReservationWindow();
            reservationWindow.Owner = this;
            reservationWindow.ReservationItem = item;
            reservationWindow.Mode = ReservationWindow.ReservationMode.Summary;
            reservationWindow.Show();
        }

        private void UpdateReservation_Click(object sender, RoutedEventArgs e)
        {
            ReservationItem item = ((FrameworkElement)sender).DataContext as ReservationItem;
            ReservationWindow reservationWindow = new ReservationWindow();
            reservationWindow.Owner = this;
            reservationWindow.ReservationItem = item;
            reservationWindow.Service = Service;
            reservationWindow.Authorization = Authorization;
            reservationWindow.Mode = ReservationWindow.ReservationMode.Edit;
            reservationWindow.Show();
        }

        private void CancelReservation_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.Ask(this, "Czy na pewno chcesz anulować wybraną rezerwację?"))
            {
                ReservationItem item = ((FrameworkElement)sender).DataContext as ReservationItem;
                CancelReservation(item.Reservation);
            }
        }

        private void Tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl && ReservationsTab.IsSelected)
                LoadReservationsData();
        }
        
        private async void LoadShowingsData()
        {
            StartLoading();
            try
            {
                EntityList<Showing> response = await Service.GetShowingsAsync();
                List<Showing> data = response.List;
                ShowingsDataGrid.ItemsSource = data;
            }
            catch (UnsuccessfulResponseException e)
            {
                Utils.ShowError(this, e.Message);
            }
            StopLoading();
        }

        private async void LoadReservationsData()
        {
            StartLoading();
            try
            {
                EntityList<Reservation> response = await Service.GetReservationsAsync(Authorization);
                List<Reservation> reservations = response.List;
                List<ReservationItem> items = new List<ReservationItem>();

                if (reservations != null && reservations.Count > 0)
                {
                    List<Showing> showings = ShowingsDataGrid.ItemsSource as List<Showing>;
                    foreach (Reservation reservation in reservations)
                    {
                        Showing foundShowing = showings.Find(showing => showing.Identifier == reservation.ShowingID);
                        items.Add(new ReservationItem(reservation, foundShowing));
                    }
                }
                ReservationsDataGrid.ItemsSource = items;
            }
            catch (UnsuccessfulResponseException e)
            {
                Utils.ShowError(this, e.Message);
            }
            StopLoading();
        }

        private async void CancelReservation(Reservation reservation)
        {
            StartLoading();
            try
            {
                await Service.CancelReservationAsync(Authorization, reservation.Identifier);
                Utils.ShowInfo(this, "Rezerwacja została pomyślnie anulowana.");
                LoadReservationsData();
            }
            catch (UnsuccessfulResponseException e)
            {
                Utils.ShowError(this, e.Message);
            }
            StopLoading();
        }
    }
}
