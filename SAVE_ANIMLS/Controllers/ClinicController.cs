using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicController : ControllerBase
    {
        private readonly IMongoCollection<Clinic> _clinicCollection;

        public ClinicController(IMongoDatabase database)
        {
            _clinicCollection = database.GetCollection<Clinic>("clinics");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clinic>>> GetClinics()
        {
            var clinics = await _clinicCollection.Find(_ => true).ToListAsync();
            return clinics;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Clinic>> GetClinic(string id)
        {
            var clinic = await _clinicCollection.Find(c => c.Id == new MongoDB.Bson.ObjectId(id)).FirstOrDefaultAsync();
            if (clinic == null)
            {
                return NotFound();
            }
            return clinic;
        }

        [HttpPost]
        public async Task<ActionResult<Clinic>> CreateClinic(Clinic clinic)
        {
            clinic.Id = ObjectId.GenerateNewId();
            await _clinicCollection.InsertOneAsync(clinic);
            return CreatedAtAction(nameof(GetClinic), new { id = clinic.Id }, clinic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClinic(string id, Clinic updatedClinic)
        {
            var filter = Builders<Clinic>.Filter.Eq(c => c.Id, new MongoDB.Bson.ObjectId(id));
            var update = Builders<Clinic>.Update
                .Set(c => c.Name, updatedClinic.Name)
                .Set(c => c.Address, updatedClinic.Address)
                .Set(c => c.OpeningTime, updatedClinic.OpeningTime)
                .Set(c => c.ClosingTime, updatedClinic.ClosingTime)
                .Set(c => c.Location, updatedClinic.Location)
                .Set(c => c.Veterinarian, updatedClinic.Veterinarian)
                .Set(c => c.Services, updatedClinic.Services);

            var options = new FindOneAndUpdateOptions<Clinic>
            {
                ReturnDocument = ReturnDocument.After
            };

            var existingClinic = await _clinicCollection.FindOneAndUpdateAsync(filter, update, options);

            if (existingClinic == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinic(string id)
        {
            var deleteResult = await _clinicCollection.DeleteOneAsync(c => c.Id == new MongoDB.Bson.ObjectId(id));
            if (deleteResult.DeletedCount == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}