using DogBreedAPI_SPP.Models;

namespace DogBreedAPI_SPP.Common
{
    public static class DogBreedCachedValues
    {
        public static List<Dog> DogBreedImagesInCache { get; private set; } = new List<Dog>(); // I like to initialize this so as to avoid null checks and null exceptions

        public static void SetDogBreedImagesCache(List<Dog> dogImagesInCache)
        {

            if (dogImagesInCache?.Count > 0)
            {
                DogBreedImagesInCache = dogImagesInCache;
            }
        }

        public static string GetDogImage(string breedName)
        {
            string result = string.Empty;

            if (!string.IsNullOrWhiteSpace(breedName) && DogBreedImagesInCache.Count > 0)
            {
                if (breedName.Contains("-"))
                    return result = GetDogSubBreedImage(breedName);
                else
                    return result = GetDogBreedImage(breedName);
            }
            return string.Empty;

        }

        private static string GetDogBreedImage(string breedName)
        {
            string result = DogBreedImagesInCache.Find(x => x.DogBreedName == breedName)?.ImageUrl;  // this is for exact matched record that is saved by breed not with subbreed

            if (string.IsNullOrWhiteSpace(result))
                result = DogBreedImagesInCache.Where(x => x.DogBreedName.Contains(breedName))?.OrderBy(x => x.Id).Select(x => x.ImageUrl).FirstOrDefault(); // this will get the first image with subreed

            return result;
        }


        private static string GetDogSubBreedImage(string subBreed) => DogBreedImagesInCache.Find(x => x.DogBreedName == subBreed)?.ImageUrl;
        



    }
}
