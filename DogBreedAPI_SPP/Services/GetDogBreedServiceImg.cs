namespace DogBreedAPI_SPP.Services
{
    public class GetDogBreedServiceImg
    {

        public  async Task<bool> GetRequest(string url)
        {

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        var data =  JObj
                    }
                    else
                    {
                        return false;

                    }
                }
            }

            return true;
        }
    }
}
