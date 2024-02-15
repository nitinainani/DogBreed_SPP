using Newtonsoft.Json.Linq;

namespace DogBreedAPI_SPP.Services
{
    public interface IGetDogBreedImgService
    {
        Task<string> GetRequest(string url);
    }

    public class GetDogBreedImgService : IGetDogBreedImgService
    {

        public async Task<string> GetRequest(string url)
        {

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        JObject data = JObject.Parse(apiResponse); // using jobject since there is not much to create for deserialization, so keeping it simple
                        var image = data["message"].ToString();
                        return string.IsNullOrWhiteSpace(image) ? string.Empty : image;
                    }
                }
            }

            return string.Empty;
        }
    }
}
