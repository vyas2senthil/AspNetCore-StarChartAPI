using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if(celestialObject == null) //If not found, then return Notfound.
            {
                return NotFound();
            }
            //Set Satellite property using orbited id parameter of all celestial objects and that which match with id.
            celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            // Get celestial object using its name.
            var celestialObjects = _context.CelestialObjects.Where(e => e.Name == name).ToList();
            if(!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach(var celestialObject in celestialObjects)
            {
                // Setting the Satellites with the CelestialObjects with Orbitted Object id.            
                celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll() {
            // Getting all the celestial objects.
            List<CelestialObject> celestialObjects = _context.CelestialObjects.ToList();

            foreach(var celestialObject in celestialObjects)
            {
                // Setting the Satellite for each celestial object.
                celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject); // Add new celestialObject to CelestialObject's context.
            _context.SaveChanges(); // Save.
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject); // 201 Response with route.
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            //var existingCelestialObject = _context.CelestialObjects.Where(e => e.Id == id).FirstOrDefault();
            var existingCelestialObject = _context.CelestialObjects.Find(id); // Get Existing Celestial object using its primary key Id.

            if (existingCelestialObject == null)
            {
                return NotFound();
            }
            // Set the properties of existing object with parameter objects' properties.
            existingCelestialObject.Name = celestialObject.Name;
            existingCelestialObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            existingCelestialObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(existingCelestialObject); // Updating the celestial object with the object provided in parameter.
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestialObject = _context.CelestialObjects.Find(id); //Get existing celestial object.
            if(celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Name = name; // Update existing cel object with name from parameter.
            _context.CelestialObjects.Update(celestialObject); // Update Celstial Objects db context.
            _context.SaveChanges(); 
            return NoContent(); // Return Nothing.
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            List<CelestialObject> celestialObjects = _context.CelestialObjects.Where(e => e.Id == id || e.OrbitedObjectId == id).ToList(); // get all celstial objects with id or orbital obj id from paramter.
            if (!celestialObjects.Any())
            {
                return NotFound();
            }
            _context.CelestialObjects.RemoveRange(celestialObjects); // Remove all celestial objects which match above.
            _context.SaveChanges();
            return NoContent();
        }


    }
}
