using Dapper;
using DogBreedAPI_SPP.Models;
using Microsoft.Data.SqlClient;

namespace DogBreedAPI_SPP.Repos
{
    public interface IDogBreedRepo
    {
        IEnumerable<Dog> GetAllBreeds();
        Task<bool> InsertRecord(Dog dog);
    }

    public class DogBreedRepo : IDogBreedRepo
    {
        private readonly string DBConnectionString;

        public DogBreedRepo(IConfiguration configuration)
        {
            DBConnectionString = configuration.GetValue<string>("DBConnection");
        }

        public IEnumerable<Dog> GetAllBreeds() {
            try
            {
                using (var connection = new SqlConnection(DBConnectionString))
                {
                    connection.Open();
                    var sql = "SELECT * FROM DogBreed";
                    var dogs = connection.Query<Dog>(sql);
                    return dogs;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //public async Task<bool> InsertRecord(Dog dog)
        //{
        //    try
        //    {
        //        using (var connection = new SqlConnection(DBConnectionString))
        //        {
        //            connection.Open();
        //            var sql = "INSERT INTO DogBreed (DogBreedName, ImageUrl, CreateDate ) VALUES (@DogBreedName, @ImageUrl,@CreateDate )";

        //            var recordToBeInserted = new { DogBreedName = dog.DogBreedName, ImageUrl = dog.ImageUrl, CreateDate = DateTime.Now };

        //            var rowsAffected = await connection.ExecuteAsync(sql, recordToBeInserted);

        //            return rowsAffected > 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}


        /*
           Upsert (Merge insert) here is the safest bet. However, the above code should be fine too
         */
        public async Task<bool> InsertRecord(Dog dog)
        {
            try
            {
                using (var connection = new SqlConnection(DBConnectionString))
                {
                    connection.Open();
                    var sql = @"  MERGE INTO DogBreed WITH (HOLDLOCK) as T
                      USING(VALUEs(@DogBreedName, @ImageUrl,@CreateDate )) AS S(DogBreedName, ImageUrl, CreateDate)
                      ON T.DogBreedName = S.DogBreedName
                      WHEN NOT MATCHED THEN  INSERT VALUES(S.DogBreedName, S.ImageUrl, S.CreateDate); ";

                    var recordToBeInserted = new { DogBreedName = dog.DogBreedName, ImageUrl = dog.ImageUrl, CreateDate = DateTime.Now };

                    var rowsAffected = await connection.ExecuteAsync(sql, recordToBeInserted);

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
