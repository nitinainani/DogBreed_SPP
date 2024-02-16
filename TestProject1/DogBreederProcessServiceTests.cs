using AutoFixture;
using DogBreedAPI_SPP.Models;
using DogBreedAPI_SPP.Repos;
using DogBreedAPI_SPP.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class DogBreederProcessServiceTests
    {
        private IConfiguration configuration;
        private readonly DogBreederProcessService dogBreederProcessService;
        private readonly Mock<ILogger<DogBreederProcessService>> _logger;
        private readonly Mock<IDogBreedRepo> _dogRepo;
        private readonly Mock<IGetDogBreedImgService> _dogBreedImgService;
        private IEnumerable<Dog> dogs;
        public DogBreederProcessServiceTests()
        {
            IFixture fixture = new Fixture();
            _logger = new Mock<ILogger<DogBreederProcessService>>();
            _dogRepo = new Mock<IDogBreedRepo>();
            _dogBreedImgService = new Mock<IGetDogBreedImgService>();

            var InMemmorySettings = new Dictionary<string, string> {
                {"DogBreedingAPIUrl", "https://test.ceo/api/breed/breedNamePlaceHolder/images/random"}
            };

            configuration = new ConfigurationBuilder()
                            .AddInMemoryCollection(InMemmorySettings)
                            .Build();
            dogBreederProcessService = new(_logger.Object, configuration, _dogBreedImgService.Object, _dogRepo.Object);
            dogs = fixture.Create<IEnumerable<Dog>>(); 
        }
        [Fact]
        public async Task Process_LoadFirstTimeList_PopulateInMemory_ItemExists()
        {
            //Arrange
            _dogRepo.Setup(x => x.GetAllBreeds()).Returns(new List<Dog> { new Dog { DogBreedName = "test", ImageUrl = "test.com" },
                                                                          new Dog { DogBreedName = "test1", ImageUrl = "test1.com" },
                                                                            new Dog { DogBreedName = "test2", ImageUrl = "test2.com" }});

            var response  = await dogBreederProcessService.Process("test");

            Assert.True(!string.IsNullOrWhiteSpace(response));

        }

        [Fact]
        public async Task Process_FailToInsert()
        {
           
            //Arrange
            _dogRepo.Setup(x => x.GetAllBreeds()).Returns(new List<Dog> { new Dog { DogBreedName = "test", ImageUrl = "test.com" },
                                                                          new Dog { DogBreedName = "test1", ImageUrl = "test1.com" },
                                                                            new Dog { DogBreedName = "test2", ImageUrl = "test2.com" }});

            _dogBreedImgService.Setup(x => x.GetRequest(It.IsAny<string>())).ReturnsAsync("testy.com");
            var dog = new Dog() { DogBreedName = "testy", ImageUrl = "testy.com", CreateDate = System.DateTime.Now };
            _dogRepo.Setup(x => x.InsertRecord(dog)).Returns(Task.FromResult(true));
           
            //Act
            var response = await dogBreederProcessService.Process("testy");

            //Assert
            Assert.True(string.IsNullOrWhiteSpace(response));

        }


        [Fact]
        public async Task Process_CouldNotFindItem_MakesApiCall_CouldNotFoundAsWell()
        {

            //Arrange
            _dogRepo.Setup(x => x.GetAllBreeds()).Returns(dogs);

            _dogBreedImgService.Setup(x => x.GetRequest(It.IsAny<string>())).ReturnsAsync("");
            var dog = new Dog() { DogBreedName = "testy", ImageUrl = "testy.com", CreateDate = System.DateTime.Now };
            _dogRepo.Setup(x => x.InsertRecord(dog)).Returns(Task.FromResult(true));

            //Act
            var response = await dogBreederProcessService.Process("testy");

            //Assert
            Assert.True(string.IsNullOrWhiteSpace(response));

        }



        [Fact]
        public async Task Process_ThrowsException()
        {

            //Arrange
            _dogRepo.Setup(x => x.GetAllBreeds()).Throws<Exception>();

            //Act
            var response = await dogBreederProcessService.Process("testy");

            //Assert
            Assert.True(string.IsNullOrWhiteSpace(response));

        }


    }
}