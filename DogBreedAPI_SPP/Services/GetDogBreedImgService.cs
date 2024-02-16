using Newtonsoft.Json.Linq;

namespace DogBreedAPI_SPP.Services
{
    public interface IGetDogBreedImgService
    {
        Task<string> GetRequest(string url);
    }

    public class GetDogBreedImgService : IGetDogBreedImgService
    {
        private readonly ILogger<GetDogBreedImgService> _logger;
        public GetDogBreedImgService(ILogger<GetDogBreedImgService> logger)
        {
            _logger = logger;
        }
        public async Task<string> GetRequest(string url)
        {
            try
            {
                //We can also use polly (It would be an overkill here so not using it) or any other service for retry mechanism so as to how many times we want to retry and in what intervals between successive attempts
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
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetDogBreedImgService with Error : {ex.Message}  and Stack trace : {ex.StackTrace}");

            }


            return string.Empty;
        }
    }
}
