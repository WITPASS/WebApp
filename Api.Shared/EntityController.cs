using Data;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Shared
{
    [ApiController, Route("api/[controller]"), Authorize]
    public class EntityController<T, T2> : ControllerBase where T : DbContext where T2 : Entity
    {
        protected readonly T _context;
        protected readonly DbSet<T2> _dbSet;

        public EntityController(T context, DbSet<T2> dbSet)
        {
            _context = context;
            _dbSet = dbSet;
        }

        [HttpGet, EnableQuery]
        virtual public IEnumerable<T2> Get()
        {
            return _dbSet.AsNoTracking();
        }

        [HttpGet("{id}"), EnableQuery]
        virtual public SingleResult<T2> Get(Guid id)
        {
            var result = _dbSet.AsNoTracking().Where(c => c.Id == id);
            return SingleResult.Create(result);
        }

        [HttpPut("{id}")]
        virtual public async Task<IActionResult> Put(Guid id, T2 ent)
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
        virtual public async Task<ActionResult<T2>> Post(T2 ent)
        {
            _dbSet.Add(ent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = ent.Id }, ent);
        }

        [HttpDelete("{id}")]
        virtual public async Task<ActionResult<T2>> Delete(Guid id)
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
}
