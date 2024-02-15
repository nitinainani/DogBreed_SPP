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

        public DogBreederProcessService(IConfiguration configuration, IGetDogBreedImgService getDogBreedServiceImg, IDogBreedRepo dogBreedRepo)
        {
            _getDogBreedServiceImg = getDogBreedServiceImg;
            apiUrl = configuration.GetValue<string>("DogBreedingAPIUrl"); // url is configurable and can be stored in environment variables source
            _dogBreedRepo = dogBreedRepo;
        }


        //public async Task<string> Process(string breedName)
        //{

        //    var finalapiUrl = apiUrl.Replace("breedNamePlaceHolder", breedName); // there is place holder in route in appsettings, that is for breedname, so it is being replaced by actual breedname

        //    var result = await _getDogBreedServiceImg.GetRequest(finalapiUrl);
        //   var answer = await _dogBreedRepo.InsertRecord(new Dog { DogBreedName = breedName, ImageUrl = result });
        //    var answer2 = _dogBreedRepo.GetAllBreeds();
        //    return result;
        //}


        public async Task<string> Process(string breedName)
        {
            string result = string.Empty;
            
            result = DogBreedCachedValues.GetDogBreedImage(breedName); // Created this static class in Common to keep track of all images that we have in db. This way we only query db when we could not find in the static list
            //The advantage of static object is it maintains single object in memory accross the app 

            if (string.IsNullOrWhiteSpace(result)) { 
                var finalapiUrl = apiUrl.Replace("breedNamePlaceHolder", breedName); // there is place holder in route in appsettings, that is for breedname, so it is being replaced by actual breedname
                result = await _getDogBreedServiceImg.GetRequest(finalapiUrl);
                var recordInserted = await _dogBreedRepo.InsertRecord(new Dog { DogBreedName = breedName, ImageUrl = result });
                if (recordInserted) DogBreedCachedValues.SetDogBreedImagesCache(_dogBreedRepo.GetAllBreeds().ToList()); // we refresh the static list once we insert new record in DB
            
            }
            return result;
        }
    }
}
