using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SAVE_ANIMLS.Models;
using System.Security.Claims;

namespace SAVE_ANIMLS.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;

        public UsersController(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("users");
        }

        [HttpGet]
        [Authorize(Roles = "AdminUser")]
        public ActionResult<IEnumerable<object>> GetUsers()
        {
            var users = _users.Find(u => true).ToList();
            var transformedUsers = users.Select(u => new
            {
                id = u.Id.ToString(),
                u.Name,
                u.Email,
                u.Password,
                u.DateOfBirth,
                u.Address,
                u.EducationLevel,
                u.Role,
                u.IsApproved
            });
            return Ok(transformedUsers);
        }

        [HttpGet]
        [Authorize(Roles = "AdminUser")]
        [Route("NotApproved")]
        public ActionResult<IEnumerable<object>> GetUsersNotApproved()
        {
            var users = _users.Find(u => u.IsApproved != true).ToList();
            var transformedUsers = users.Select(u => new
            {
                id = u.Id.ToString(),
                u.Name,
                u.Email,
                u.Password,
                u.DateOfBirth,
                u.Address,
                u.EducationLevel,
                u.Role,
                u.IsApproved
            });
            return Ok(transformedUsers);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "AdminUser")]
        public ActionResult<User> GetUser(string id)
        {
            var user = _users.Find(u => u.Id == new MongoDB.Bson.ObjectId(id)).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateUser(string id, User updatedUser)
        {
            var userIdentity = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "AdminUser" || userIdentity == id)
            {
                var user = _users.FindOneAndUpdate(u => u.Id == new MongoDB.Bson.ObjectId(id),
                    Builders<User>.Update.Set(u => u.Name, updatedUser.Name)
                                         .Set(u => u.Email, updatedUser.Email)
                                         .Set(u => u.Password, updatedUser.Password)
                                         .Set(u => u.DateOfBirth, updatedUser.DateOfBirth)
                                         .Set(u => u.Address, updatedUser.Address)
                                         .Set(u => u.EducationLevel, updatedUser.EducationLevel)
                                         .Set(u => u.Role, updatedUser.Role)
                                         .Set(u => u.IsApproved, updatedUser.IsApproved));

                if (user == null)
                {
                    return NotFound();
                }

                return NoContent();
            }

            return Unauthorized();
        }

        [HttpPut("approve/{id}")]
        [Authorize(Roles = "AdminUser")]
        public IActionResult ApproveAccount(string id)
        {
            var user = _users.FindOneAndUpdate(
                u => u.Id == new ObjectId(id),
                Builders<User>.Update.Set(u => u.IsApproved, true));

            if (user == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "AdminUser")]
        public IActionResult DeleteUser(string id)
        {
            var result = _users.DeleteOne(u => u.Id == new MongoDB.Bson.ObjectId(id));
            if (result.DeletedCount == 0)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("UploadImage/{id}")]
        [Authorize]
        public async Task<IActionResult> UploadImage(string id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Generate a unique file name
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

            // Define the path where the uploaded image will be saved
            string imagePath = Path.Combine("uploads\\Images\\", uniqueFileName);

            // Save the uploaded image to the specified path
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update the ImageProfilePath property in the user model
            // Replace with the actual user ID
            var user = _users.FindOneAndUpdate(u => u.Id == new MongoDB.Bson.ObjectId(id),
                    Builders<User>.Update.Set(u => u.ImageProfile, imagePath));

            return Ok("Image uploaded successfully.");
        }
    }
}