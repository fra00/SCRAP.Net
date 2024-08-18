using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MainRobot.Http
{
    public class RestApiClient
    {
        // Crea un'istanza di HttpClient e configura le intestazioni
        private readonly HttpClient client;

        public RestApiClient(string baseUrl)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "MyApp");
        }

        public void addHeader(string key, string value) {
            client.DefaultRequestHeaders.Add(key, value);
        }

        // Invia una richiesta HTTP in base al metodo specificato
        public async Task<HttpResponseMessage> SendRequestAsync(string method, string url, object data = null)
        {
            // Crea un oggetto HttpRequestMessage con il metodo e l'URL
            HttpRequestMessage request = new(new HttpMethod(method), url);

            // Se c'è un dato da inviare, serializzalo in JSON e aggiungilo al contenuto della richiesta
            if (data != null)
            {
                string json = JsonSerializer.Serialize(data);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            // Invia la richiesta asincrona e ottieni la risposta
            HttpResponseMessage response = await client.SendAsync(request);

            // Restituisci la risposta
            return response;
        }
    }
}