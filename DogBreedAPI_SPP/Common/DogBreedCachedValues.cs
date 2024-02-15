using DogBreedAPI_SPP.Models;

namespace DogBreedAPI_SPP.Common
{
    public static class DogBreedCachedValues
    {
        public static List<Dog> DogBreedImagesInCache { get; private set; } = new List<Dog>() { new Dog { DogBreedName = "affenpinscher", ImageUrl = "https://images.dog.ceo/breeds/affenpinscher/n02110627_11614.jpg" } }; // I like to initialize this so as to avoid null checks and null exceptions

        public static void SetDogBreedImagesCache(List<Dog> dogImagesInCache) {

            if (dogImagesInCache?.Count > 0) {
                DogBreedImagesInCache = dogImagesInCache;
            }
        }

        public static string GetDogBreedImage(string breedName) {

            if (!string.IsNullOrWhiteSpace(breedName) && DogBreedImagesInCache.Count > 0) {
               return DogBreedImagesInCache.Find(x => x.DogBreedName == breedName)?.ImageUrl;
            }
            return string.Empty;  
        }
                 

    
    }
}
