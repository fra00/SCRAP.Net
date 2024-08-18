namespace MainRobot.Robot.Comunication.ComunicationTransport.HttpClient
{
    public class HttpClientComunication : IHttpClientComunication
    {
        private IHttpClientFactory httpClientFactory;

        public HttpClientComunication(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<string> SendAsync(string command)
        {
            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(Configuration.HTTP_URL_COMUNICATION + command);
            string content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }

}
