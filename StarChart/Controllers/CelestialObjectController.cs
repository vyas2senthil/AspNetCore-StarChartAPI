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


    }
}
