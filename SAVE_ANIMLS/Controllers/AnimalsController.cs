using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace SAVE_ANIMLS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly IMongoCollection<AnimalModel> _animalCollection;

        public AnimalsController(IMongoDatabase database)
        {
            _animalCollection = database.GetCollection<AnimalModel>("animals");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnimalModel>>> GetAnimals()
        {
            var animals = await _animalCollection.Find(_ => true).ToListAsync();
            return Ok(animals);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AnimalModel>> GetAnimal(string id)
        {
            var animal = await _animalCollection.Find(a => a.Id == new ObjectId(id)).FirstOrDefaultAsync();
            if (animal == null)
            {
                return NotFound();
            }
            return Ok(animal);
        }

        [HttpPost]
        public async Task<ActionResult<AnimalModel>> CreateAnimal(AnimalModel animal)
        {
            await _animalCollection.InsertOneAsync(animal);
            return CreatedAtAction(nameof(GetAnimal), new { id = ObjectId.GenerateNewId() }, animal);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnimal(string id, AnimalModel updatedAnimal)
        {
            var animal = await _animalCollection.FindOneAndUpdateAsync(
                Builders<AnimalModel>.Filter.Eq(a => a.Id, new ObjectId(id)),
                Builders<AnimalModel>.Update.Set(a => a.Name, updatedAnimal.Name)
                                            .Set(a => a.Species, updatedAnimal.Species)
                                            .Set(a => a.Age, updatedAnimal.Age)
                                            .Set(a => a.Gender, updatedAnimal.Gender)
                                            .Set(a => a.Description, updatedAnimal.Description)
                                            .Set(a => a.Address, updatedAnimal.Address)
                                            .Set(a => a.MedicalCondition, updatedAnimal.MedicalCondition)
                                            .Set(a => a.Location, updatedAnimal.Location)
                                            .Set(a => a.RescueStatus, updatedAnimal.RescueStatus),
                new FindOneAndUpdateOptions<AnimalModel> { ReturnDocument = ReturnDocument.After });

            if (animal == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimal(string id)
        {
            var result = await _animalCollection.DeleteOneAsync(a => a.Id == new ObjectId(id));
            if (result.DeletedCount == 0)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAnimal(string id, [FromBody] JsonPatchDocument<AnimalModel> patchDocument)
        {
            var animal = await _animalCollection.Find(a => a.Id == new ObjectId(id)).FirstOrDefaultAsync();
            if (animal == null)
            {
                return NotFound();
            }

            try
            {
                patchDocument.ApplyTo(animal);
            }
            catch (JsonPatchException ex)
            {
                return BadRequest(ex);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _animalCollection.ReplaceOneAsync(a => a.Id == new ObjectId(id), animal);

            return NoContent();
        }
    }
}