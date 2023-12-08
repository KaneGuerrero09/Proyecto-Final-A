using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Bienvenido a Kaneglo!");
        Console.Write("Ingrese su búsqueda: ");
        string query = Console.ReadLine();

        // Reemplaza 'TU_CLAVE_DE_API' con tu clave de API de Microsoft Azure.
        string apiKey = "TU_CLAVE_DE_API";

        // Realiza la búsqueda y muestra los resultados.
        await BuscarEnKaneglo(query, apiKey);

        Console.ReadLine();
    }

    static async Task BuscarEnKaneglo(string query, string apiKey)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            string url = $"https://api.cognitive.microsoft.com/bing/v7.0/search?q={Uri.EscapeDataString(query)}&count=5";

            // Agrega la clave de API a los encabezados de la solicitud.
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

            try
            {
                // Realiza la solicitud a la API de Bing Search.
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Lanza una excepción si hay un error.

                // Lee y procesa la respuesta JSON.
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    JsonDocument jsonDocument = await JsonDocument.ParseAsync(responseStream);
                    MostrarResultados(jsonDocument);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error de solicitud: {e.Message}");
            }
        }
    }

    static void MostrarResultados(JsonDocument jsonDocument)
    {
        JsonElement root = jsonDocument.RootElement;

        if (root.TryGetProperty("webPages", out JsonElement webPages) && webPages.TryGetProperty("value", out JsonElement value))
        {
            foreach (JsonElement resultado in value.EnumerateArray())
            {
                Console.WriteLine($"Título: {resultado.GetProperty("name").GetString()}");
                Console.WriteLine($"URL: {resultado.GetProperty("url").GetString()}");
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("No se encontraron resultados.");
        }
    }
}
