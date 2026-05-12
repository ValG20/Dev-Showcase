namespace AppWebEditorial.Models
{
    public class ApiEditorial
    {
        public HttpClient IniciarApi()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7132/");
            return client;
        }

    }
}
