using DogBreedAPI_SPP.Common;
using DogBreedAPI_SPP.Models;
using DogBreedAPI_SPP.Repos;

namespace DogBreedAPI_SPP.Services
{
    public interface IDogBreederProcessService
    {
        Task<string> Process(string breedName);
    }

    public class DogBreederProcessService : IDogBreederProcessService
    {
        private readonly IGetDogBreedImgService _getDogBreedServiceImg;
        private readonly string apiUrl;
        private readonly IDogBreedRepo _dogBreedRepo;
        private readonly ILogger<DogBreederProcessService> _logger;

        public DogBreederProcessService(ILogger<DogBreederProcessService> logger,IConfiguration configuration, IGetDogBreedImgService getDogBreedServiceImg, IDogBreedRepo dogBreedRepo)
        {
            _logger = logger;
            _getDogBreedServiceImg = getDogBreedServiceImg;
            apiUrl = configuration.GetValue<string>("DogBreedingAPIUrl"); // url is configurable and can be stored in environment variables source
            _dogBreedRepo = dogBreedRepo;
        }


        public async Task<string> Process(string breedName)
        {
            try
            {
                string result = string.Empty;

                if (DogBreedCachedValues.DogBreedImagesInCache.Count == 0) // This is the first time when the static list of dog breed images is empty we want to fill it
                    DogBreedCachedValues.SetDogBreedImagesCache(_dogBreedRepo.GetAllBreeds().ToList());

                result = DogBreedCachedValues.GetDogImage(breedName); // Created this static class in Common to keep track of all images that we have in db. This way we only query db when we could not find in the static list
                                                                           //The advantage of static object is, it maintains single object in memory accross the app 

                if (string.IsNullOrWhiteSpace(result))
                {

                    result = await MakeApiCallAndRefreshImageCache(breedName);
                }
                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while process request for dog breed {breedName} with Error : {ex.Message} and Stack trace : {ex.StackTrace}");
                return String.Empty;
            }
            
        }


        private async Task<string> MakeApiCallAndRefreshImageCache(string breedName) {
            string result = string.Empty;
            string finalApiUrl = string.Empty;
            if (breedName.Contains("-")) { 
                string[] dogBreedAndSubreed = breedName.Split('-');
                finalApiUrl = apiUrl.Replace("breedNamePlaceHolder",@$"{dogBreedAndSubreed[0]}/{dogBreedAndSubreed[1]}");
            }
            else 
            finalApiUrl = apiUrl.Replace("breedNamePlaceHolder", breedName); // there is place holder in route in appsettings, that is for breedname, so it is being replaced by actual breedname
            

            result = await _getDogBreedServiceImg.GetRequest(finalApiUrl);
            bool recordInserted = false;
            if (!string.IsNullOrWhiteSpace(result))
                recordInserted = await _dogBreedRepo.InsertRecord(new Dog { DogBreedName = breedName, ImageUrl = result });

            if (recordInserted) DogBreedCachedValues.SetDogBreedImagesCache(_dogBreedRepo.GetAllBreeds().ToList()); // we refresh the static list once we insert new record in DB

            result = DogBreedCachedValues.GetDogImage(breedName); // Created this static class in Common to keep track of all images that we have in db. This way we only query db when we could not find in the static list
            return result;
        }
    }
}
