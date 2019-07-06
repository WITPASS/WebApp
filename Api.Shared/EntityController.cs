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
    public class EntityController<T, T2> : ControllerBase where T : Entity where T2 : DbContext
    {
        protected readonly T2 _context;
        protected readonly DbSet<T> _dbSet;

        public EntityController(T2 context, DbSet<T> dbSet)
        {
            _context = context;
            _dbSet = dbSet;
        }

        [HttpGet, EnableQuery]
        virtual public IEnumerable<T> Get()
        {
            return _dbSet.AsNoTracking();
        }

        [HttpGet("{id}"), EnableQuery]
        virtual public SingleResult<T> Get(Guid id)
        {
            var result = _dbSet.AsNoTracking().Where(c => c.Id == id);
            return SingleResult.Create(result);
        }

        [HttpPut("{id}")]
        virtual public async Task<IActionResult> Put(Guid id, T ent)
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
        virtual public async Task<ActionResult<T>> Post(T ent)
        {
            _dbSet.Add(ent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = ent.Id }, ent);
        }

        [HttpDelete("{id}")]
        virtual public async Task<ActionResult<T>> Delete(Guid id)
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
