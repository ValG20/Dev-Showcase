namespace AppWebSeguridad.Models
{
    public class ApiSeguridad
    {
        public HttpClient IniciarApi()
        {

            //Variable para manejar la referencia con la API
            HttpClient client = new HttpClient();

            //Se indica la API local 
            client.BaseAddress = new Uri("https://localhost:7187/");

            //Se retorna la referencia
            return client;
        }
    }
}
