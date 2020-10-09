using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoWebAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        readonly PersonsContext db;
        public PersonController(PersonsContext context)
        {
            db = context;
            if (!db.Persons.Any())
            {
                var path = Path.Combine(Environment.CurrentDirectory, "Avatars");
                db.Persons.Add(new Person { Name = "Катерина", Surname = "Чернова", Patronymic = "Сергеевна" });
                db.Photos.Add(new Photo { Id = 1, IdPerson = 1, ImageData = System.IO.File.ReadAllBytes(Path.Combine(path, "black1.jpg")) });
                db.Photos.Add(new Photo { Id = 2, IdPerson = 1, ImageData = System.IO.File.ReadAllBytes(Path.Combine(path, "black2.jpg")) });
                db.Photos.Add(new Photo { Id = 3, IdPerson = 1, ImageData = System.IO.File.ReadAllBytes(Path.Combine(path, "black3.jpg")) });
                db.SaveChanges();
                db.Persons.Add(new Person { Name = "Сергей", Surname = "Серов", Patronymic = "Сергеевич" });
                db.Photos.Add(new Photo { Id = 1, IdPerson = 2, ImageData = System.IO.File.ReadAllBytes(Path.Combine(path, "grey.jpg")) });
                db.SaveChanges();
                db.Persons.Add(new Person { Name = "Иван", Surname = "Рыженков", Patronymic = "Иванович" });
                db.Photos.Add(new Photo { Id = 1, IdPerson = 3, ImageData = System.IO.File.ReadAllBytes(Path.Combine(path, "red.jpg")) });
                db.SaveChanges();
            }
        }

        // GET api/person
        [HttpGet]
        public async Task<ActionResult<List<PersonDTO>>> Get()
        {
            return await db.Persons
                .Select(person => new PersonDTO(person))
                .ToListAsync();
        }

        // GET api/person/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonDTO>> GetPerson(int id)
        {
            var person = await db.Persons.FindAsync(id);
            if (person == null)
                return NotFound();
            return new ObjectResult(new PersonDTO(person));
        }

        // GET api/person/2/face/1
        [HttpGet("{id}/face/{idFace}")]
        public async Task<IActionResult> GetFaceOfPerson(int id, int idFace)
        {
            var photo =await db.Photos.FindAsync(id,idFace);
            if (photo == null)
                return NotFound();
            return File(photo.ImageData, "image/jpeg");
        }

        // GET api/person/find_by_photo
        [HttpGet("find_by_photo")]
        public async Task<ActionResult<PersonDTO>> FindPersonByFace(IFormFile file)
        {
            if (!Photo.IsFileOK(file).Item1)
                return BadRequest(Photo.IsFileOK(file).Item2);
            byte[] buffer = Photo.ReadBytesOfFile(file);
            int IdPerson = 0;
            foreach (var photo in db.Photos)
                if (photo.ImageData.SequenceEqual(buffer))
                {
                    IdPerson = photo.IdPerson;
                    break;
                }
            if (IdPerson == 0)
                return NotFound();
            return new PersonDTO(await db.Persons.FindAsync(IdPerson));
        }

        // POST api/person
        [HttpPost]
        public async Task<ActionResult> PostPerson([FromForm] Person person, IFormFile file)
        {
            if (person == null)
                return BadRequest();
            if (!Photo.IsFileOK(file).Item1)
                return BadRequest(Photo.IsFileOK(file).Item2);
            db.Persons.Add(person);
            await db.SaveChangesAsync();
            db.Photos.Add(new Photo(1, person.Id, file));
            await db.SaveChangesAsync();
            return Ok();
        }

        // POST api/person/5/face
        [HttpPost("{id}/face")]
        public async Task<IActionResult> PostFace(int id, IFormFile file)
        {
            if (!Photo.IsFileOK(file).Item1)
                return BadRequest(Photo.IsFileOK(file).Item2);
            if (await db.Persons.FindAsync(id) == null)
                return NotFound($"Person Id ={id} not found");
            int newIdPhoto = 1;
            var photosOfPerson = db.Photos.Where(p => p.IdPerson == id);
            while (photosOfPerson.Any(p => p.Id == newIdPhoto)) //Search for an empty Id
                newIdPhoto++;
            db.Photos.Add(new Photo(newIdPhoto, id, file));
            await db.SaveChangesAsync();
            return Ok();
        }

        // PUT api/person
        [HttpPut]
        public async Task<ActionResult> PutPerson([FromForm] Person person)
        {
            if (person == null)
                return BadRequest();
            if (!db.Persons.Any(x => x.Id == person.Id))
                return NotFound($"Person Id ={person.Id} not found");
            db.Update(person);
            await db.SaveChangesAsync();
            return Ok();
        }

        // PUT api/person/1/face/2
        [HttpPut("{id}/face/{idFace}")]
        public async Task<IActionResult> PutFace(int id, int idFace, IFormFile file)
        {
            if (!Photo.IsFileOK(file).Item1)
                return BadRequest(Photo.IsFileOK(file).Item2);
            var photo = await db.Photos.FindAsync(id,idFace);
            if (photo == null)
                return NotFound();
            photo.ImageData = Photo.ReadBytesOfFile(file);           
            await db.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/person/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Person>> DeletePerson(int id)
        {
            Person person = await db.Persons.FindAsync(id);
            if (person == null)
                return NotFound();
            foreach (var photo in db.Photos.Where(p => p.IdPerson==id))
                db.Photos.Remove(photo); //delete all photos of this person
            db.Persons.Remove(person); //delete the person
            await db.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/person/1/face/2
        [HttpDelete("{id}/face/{idFace}")]
        public async Task<IActionResult> DeleteFace(int id, int idFace)
        {
            var photo = await db.Photos.FindAsync(id,idFace);
            if (photo == null)
                return NotFound();
            if (db.Photos.Where(p => p.IdPerson==id).ToList().Count<2)
                return BadRequest("You cannot delete a single photo of this person");
            db.Photos.Remove(photo);
            await db.SaveChangesAsync();
            return Ok();
        }
    }
}
