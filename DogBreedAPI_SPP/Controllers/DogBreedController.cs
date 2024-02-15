using DogBreedAPI_SPP.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DogBreedAPI_SPP.Controllers
{
    //[Route("api/[controller]")]  by default this will take the name of the router, here if we refactor the code and
                                // change the controller name all apis making call will need to be updated as well

    [Route("api/DogBreed")] // this is hardcoding the route, the advantage of this is if we refactor and change
    ///the controller name, clients making api call will not need to change the route name in the url
    [ApiController]
    public class DogBreedController : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<DogBreedDTO>> GetDogImageByBreed(string breed)
        {

            var result = await GetRequest("https://dog.ceo/api/breed/affenpinscher/images/random");

            return new List<DogBreedDTO>() { new DogBreedDTO { ImageUrl = "https://images.dog.ceo/breeds/hound-english/n02089973_3265.jpg" }, new DogBreedDTO { ImageUrl = "https://images.dog.ceo/breeds/hound-english/n02089973_4359.jpg" }, new DogBreedDTO { ImageUrl = "https://images.dog.ceo/breeds/hound-ibizan/n02091244_1283.jpg" } };
        }


        //[HttpGet]
        //public IEnumerable<string> GetDogImageByBreed(string breed)
        //{

        //    //var result = await GetRequest("https://dog.ceo/api/breed/affenpinscher/images/random");

        //    return new List<string>() { "https://images.dog.ceo/breeds/hound-english/n02089973_3265.jpg" };
        //}

        private async Task<bool> GetRequest(string url)
        {

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
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
