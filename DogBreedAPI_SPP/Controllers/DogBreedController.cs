using DogBreedAPI_SPP.Models.Dto;
using DogBreedAPI_SPP.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DogBreedAPI_SPP.Controllers
{
    //[Route("api/[controller]")]  by default this will take the name of the router, here if we refactor the code and
                                // change the controller name all apis making call will need to be updated as well

    [Route("api/DogBreed")] // this is hardcoding the route, the advantage of this is if we refactor and change
    ///the controller name, clients making api call will not need to change the route name in the url
    [ApiController]
    public class DogBreedController : ControllerBase
    {
        private readonly IDogBreederProcessService _dogBreederProcessService;
        private readonly ILogger<DogBreedController> _logger;

        public DogBreedController(ILogger<DogBreedController> logger,IDogBreederProcessService dogBreederProcessService)
        {
            _dogBreederProcessService = dogBreederProcessService;
            _logger = logger;
        }



        //[HttpGet]  I
        //public async Task<IEnumerable<DogBreedDTO>> GetDogImageByBreed(string breed)
        //{
        //    var result = await _dogBreederProcessService.Process(breed.ToLower());
        //We can use Automapper to map object to DTO, here I just wanted to show the use of DTO. The benefit of DTO is instead of returning all properties of object we just expose what is needed
        //    return new List<DogBreedDTO>() {  new DogBreedDTO { ImageUrl = result}  }};
        //}


        //If we don't put attribute HttpGet verb then by default it defaults to [HttpGet]
        [HttpGet("{dogBreed}")]
        //[ProducesResponseType(200)] either by code
        [ProducesResponseType(StatusCodes.Status200OK)] // good for readability
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<DogBreedDTO>>> GetDogImageByBreed(string dogBreed)
        {

            //We can also use MediatR and CQRS to inprove scalability and Performance (I have used both in the past and I am rusty, that's why did not use it)
            try
            {
                _logger.LogInformation($"Request received at GetDogImageByBreed with queryparameter {dogBreed}");
                if (string.IsNullOrWhiteSpace(dogBreed.Trim()))
                {
                    _logger.LogInformation($"Request received at GetDogImageByBreed with queryparameter {dogBreed}  is bad request");
                    return BadRequest();
                }
                var result = await _dogBreederProcessService.Process(dogBreed.Trim().ToLower());

                if (string.IsNullOrWhiteSpace(result))
                {
                    _logger.LogInformation($"Resource not found at GetDogImageByBreed with queryparameter {dogBreed}");
                    return NotFound();
                }

                _logger.LogInformation($"Request completed successfully for GetDogImageByBreed with queryparameter {dogBreed} ");
                //We can use Automapper to map object to DTO, here I just wanted to show the use of DTO. The benefit of DTO is instead of returning all properties of object we just expose what is needed
                return Ok(new List<DogBreedDTO>() { new DogBreedDTO { ImageUrl = result } });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at GetDogImageByBreed with queryparameter {dogBreed} with Error : {ex.Message}  and StackTrace : {ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
