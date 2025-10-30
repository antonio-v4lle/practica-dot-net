using System.Net.Http.Headers;
using System.Text.Json;

namespace ThreadingBasico.Moderno
{
    public class Pet
    {
        public long id { get; set; }
        public string? name { get; set; }
        public Category? category { get; set; }
        public List<string>? photoUrls { get; set; }
        public List<Tag>? tags { get; set; }
        public string? status { get; set; }
    }

    public class Category
    {
        public long id { get; set; }
        public string? name { get; set; }
    }

    public class Tag
    {
        public long id { get; set; }
        public string? name { get; set; }
    }
    class AsyncAPIConsumer
    {
        public static async Task NavigateAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // PetStore API endpoint
                    string url = "https://petstore.swagger.io/v2/pet/findByStatus?status=available";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync(url); ;
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine($"Response: {responseBody}");

                    // Deserialize the JSON response into a list of pets
                    var pets = JsonSerializer.Deserialize<List<Pet>>(responseBody);
                    Console.WriteLine("", pets.Count);
                    // Iterate through the list of pets and display their details
                    foreach (var pet in pets)
                    {
                        //Console.WriteLine($"Pet ID: {pet.id}, Name: {pet.name}");
                        if (pet.id.ToString().Length > 4)
                        {
                            Console.WriteLine($"Pet ID: {pet.id}, Name: {pet.name}");
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
            }
        }
    }
}