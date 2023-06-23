using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAVE_ANIMLS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IMongoCollection<ReportModel> _reportCollection;

        public ReportsController(IMongoDatabase database)
        {
            _reportCollection = database.GetCollection<ReportModel>("reports");
        }

        // GET: api/Reports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportModel>>> GetReports()
        {
            var reports = await _reportCollection.Find(_ => true).ToListAsync();
            return reports;
        }

        // GET: api/Reports/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportModel>> GetReport(string id)
        {
            var report = await _reportCollection.Find(r => r.Id == new ObjectId(id)).FirstOrDefaultAsync();
            if (report == null)
            {
                return NotFound();
            }
            return report;
        }

        // POST: api/Reports
        [HttpPost]
        public async Task<IActionResult> CreateReport(ReportModel report)
        {
            await _reportCollection.InsertOneAsync(report);
            return CreatedAtAction(nameof(GetReport), new { id = ObjectId.GenerateNewId() }, report);
        }

        // PUT: api/Reports/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(string id, ReportModel updatedReport)
        {
            var existingReport = await _reportCollection.Find(r => r.Id == new ObjectId(id)).FirstOrDefaultAsync();
            if (existingReport == null)
            {
                return NotFound();
            }

            updatedReport.Id = existingReport.Id;
            await _reportCollection.ReplaceOneAsync(r => r.Id == existingReport.Id, updatedReport);

            return NoContent();
        }

        // PATCH: api/Reports/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateReportPartial(string id, [FromBody] JsonPatchDocument<ReportModel> patchDocument)
        {
            var existingReport = await _reportCollection.Find(r => r.Id == new ObjectId(id)).FirstOrDefaultAsync();
            if (existingReport == null)
            {
                return NotFound();
            }

            patchDocument.ApplyTo(existingReport);
            await _reportCollection.ReplaceOneAsync(r => r.Id == existingReport.Id, existingReport);

            return NoContent();
        }

        // DELETE: api/Reports/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(string id)
        {
            var result = await _reportCollection.DeleteOneAsync(r => r.Id == new ObjectId(id));
            if (result.DeletedCount == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
