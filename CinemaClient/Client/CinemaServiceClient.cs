using CinemaClient.Client.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CinemaClient.Client
{
    public class CinemaServiceClient
    {
        private const string Host = "192.168.0.107:8080";
        private const string BaseURL = "http://" + Host + "/cinema/";
        private HttpClient client;

        public CinemaServiceClient()
        {
            client = new HttpClient();
        }

        public async Task<EntityList<Showing>> GetShowingsAsync()
        {
            HttpRequestMessage request = CreateRequest("showings", HttpMethod.Get);
            EntityList<Showing> list = await ProcessListRequest<Showing>(request);
            if (list == null)
                list = new EntityList<Showing>();
            return list;
        }

        public async Task<Showing> GetShowingAsync(int identifier)
        {
            HttpRequestMessage request = CreateRequest("showings/" + identifier.ToString(), HttpMethod.Get);
            return await ProcessRequest<Showing>(request);
        }

        public async Task<Film> GetFilmAsync(int identifier)
        {
            HttpRequestMessage request = CreateRequest("films/" + identifier.ToString(), HttpMethod.Get);
            Film film = await ProcessRequest<Film>(request);

            string coverUrl = film.Links.Where(item => item.Rel.Equals("cover")).First().Url;
            HttpRequestMessage coverRequest = CreateRequestWithURL(coverUrl, HttpMethod.Get);
            byte[] coverData = await ProcessDataRequest(coverRequest);

            film.Cover = coverData;
            return film;
        }

        public async Task<EntityList<Seat>> GetTakenSeatsAsync(int showingID)
        {
            HttpRequestMessage request = CreateRequest("showings/" + showingID.ToString() + "/seats", HttpMethod.Get);
            EntityList<Seat> list = await ProcessListRequest<Seat>(request);
            if (list == null)
                list = new EntityList<Seat>();
            return list;
        }

        public async Task CreateUserAsync(Model.Authorization authorization)
        {
            HttpRequestMessage request = CreateRequest("register", HttpMethod.Post, authorization);
            await ProcessNoResponseRequest(request);
        }

        public async Task LoginAsync(Model.Authorization authorization)
        {
            HttpRequestMessage request = CreateRequest("login", HttpMethod.Post, authorization);
            await ProcessNoResponseRequest(request);
        }

        public async Task<EntityList<Reservation>> GetReservationsAsync(Model.Authorization authorization)
        {
            HttpRequestMessage request = CreateRequest("reservations", HttpMethod.Get, authorization);
            EntityList<Reservation> list = await ProcessListRequest<Reservation>(request);
            if (list == null)
                list = new EntityList<Reservation>();
            return list;
        }

        public async Task<Reservation> GetReservationAsync(int identifier, Model.Authorization authorization)
        {
            HttpRequestMessage request = CreateRequest("reservations/" + identifier.ToString(), HttpMethod.Get, authorization);
            return await ProcessRequest<Reservation>(request);
        }

        public async Task<string> MakeReservationAsync(Model.Authorization authorization, Reservation reservation)
        {
            HttpRequestMessage request = CreateRequest("reservations/", HttpMethod.Post, authorization, reservation);
            return await ProcessStringRequest(request);
        }

        public async Task CancelReservationAsync(Model.Authorization authorization, int identifier)
        {
            HttpRequestMessage request = CreateRequest("reservations/" + identifier.ToString(), HttpMethod.Delete, authorization);
            await ProcessNoResponseRequest(request);
        }

        public async Task UpdateReservationAsync(Model.Authorization authorization, int identifier, Reservation reservation)
        {
            HttpRequestMessage request = CreateRequest("reservations/" + identifier.ToString(), HttpMethod.Put, authorization, reservation);
            await ProcessNoResponseRequest(request);
        }

        private HttpRequestMessage CreateRequestWithURL(string url, HttpMethod httpMethod)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(url);
            request.Method = httpMethod;
            request.Headers.Add("IP-Address", GetIPAddress());
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Host", Host);
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Connection", "keep-alive");
            return request;
        }

        private HttpRequestMessage CreateRequest(string uri, HttpMethod httpMethod)
        {
            return CreateRequestWithURL(BaseURL + uri, httpMethod);
        }

        private HttpRequestMessage CreateRequest(string uri, HttpMethod httpMethod, Model.Authorization authorization)
        {
            HttpRequestMessage request = CreateRequest(uri, httpMethod);
            request.Headers.Add("Authorization", authorization.CreateHeader());
            return request;
        }

        private HttpRequestMessage CreateRequest<T>(string uri, HttpMethod httpMethod, Model.Authorization authorization, T entity)
        {
            HttpRequestMessage request = CreateRequest(uri, httpMethod, authorization);
            string json = JsonConvert.SerializeObject(entity);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return request;
        }

        private async Task<EntityList<T>> ProcessListRequest<T>(HttpRequestMessage request)
        {
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                dynamic data = JObject.Parse(json);
                if (data.list is JObject)
                {
                    List<T> list = new List<T>();
                    list.Add((data.list as JObject).ToObject<T>());
                    EntityList<T> result = new EntityList<T>(list);
                    return result;
                }
                return (data as JObject).ToObject<EntityList<T>>();
            }
            else
            {
                string message = await response.Content.ReadAsStringAsync();
                message = string.IsNullOrEmpty(message) ? response.ReasonPhrase : message;
                throw new UnsuccessfulResponseException((int)response.StatusCode, message);
            }
        }

        private async Task<T> ProcessRequest<T>(HttpRequestMessage request)
        {
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            else
            {
                string message = await response.Content.ReadAsStringAsync();
                message = string.IsNullOrEmpty(message) ? response.ReasonPhrase : message;
                throw new UnsuccessfulResponseException((int)response.StatusCode, message);
            }
        }

        private async Task<byte[]> ProcessDataRequest(HttpRequestMessage request)
        {
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsByteArrayAsync();
            else
            {
                string message = await response.Content.ReadAsStringAsync();
                message = string.IsNullOrEmpty(message) ? response.ReasonPhrase : message;
                throw new UnsuccessfulResponseException((int)response.StatusCode, message);
            }
        }

        private async Task<string> ProcessStringRequest(HttpRequestMessage request)
        {
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
            {
                string message = await response.Content.ReadAsStringAsync();
                message = string.IsNullOrEmpty(message) ? response.ReasonPhrase : message;
                throw new UnsuccessfulResponseException((int)response.StatusCode, message);
            }
        }

        private async Task ProcessNoResponseRequest(HttpRequestMessage request)
        {
            HttpResponseMessage response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                string message = await response.Content.ReadAsStringAsync();
                message = string.IsNullOrEmpty(message) ? response.ReasonPhrase : message;
                throw new UnsuccessfulResponseException((int)response.StatusCode, message);
            }
        }

        private string GetIPAddress()
        {
            string hostName = Dns.GetHostName();
            return Dns.GetHostByName(hostName).AddressList[0].MapToIPv4().ToString();
        }
    }
}
