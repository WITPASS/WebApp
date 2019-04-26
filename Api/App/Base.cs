using Data;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<T> : ControllerBase where T : Entity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseController(AppDbContext context, DbSet<T> dbSet)
        {
            _context = context;
            _dbSet = dbSet;
        }

        [HttpGet, EnableQuery]
        public async Task<ActionResult<IEnumerable<T>>> Get()
        {
            return await _dbSet.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<T>> Get(Guid id)
        {
            var ent = await _dbSet.FindAsync(id);

            if (ent == null)
            {
                return NotFound();
            }

            return ent;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, T ent)
        {
            if (id != ent.Id)
            {
                return BadRequest();
            }

            _context.Entry(ent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<T>> Post(T ent)
        {
            _dbSet.Add(ent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = ent.Id }, ent);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<T>> Delete(Guid id)
        {
            var ent = await _dbSet.FindAsync(id);
            if (ent == null)
            {
                return NotFound();
            }

            _dbSet.Remove(ent);
            await _context.SaveChangesAsync();

            return ent;
        }

        private bool Exists(Guid id)
        {
            return _dbSet.Any(e => e.Id == id);
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class WelcomeController : ControllerBase
    {
        protected readonly AppDbContext _context;
        public WelcomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public object Get()
        {
            return new { Message = "Welcome" };
        }

        [HttpGet, Route("update-database")]
        public async Task<string> UpdateDatabase()
        {
            await _context.Database.MigrateAsync();
            return "success";
        }
    }
}
