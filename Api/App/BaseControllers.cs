using Data;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize]
    public class EntityController<T> : ControllerBase where T : Entity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public EntityController(AppDbContext context, DbSet<T> dbSet)
        {
            _context = context;
            _dbSet = dbSet;
        }

        [HttpGet, EnableQuery]
        virtual public IEnumerable<T> Get()
        {
            return _dbSet.AsNoTracking();
        }

        [HttpGet("{id}")]
        virtual public async Task<ActionResult<T>> Get(Guid id)
        {
            var ent = await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            if (ent == null)
            {
                return NotFound();
            }

            return ent;
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

    public class BranchEntityController<T> : EntityController<T> where T : BranchEntity
    {
        public BranchEntityController(AppDbContext context, DbSet<T> dbSet) : base(context, dbSet) { }

        public override IEnumerable<T> Get()
        {
            return _dbSet.AsNoTracking().Where(c => c.BranchId == BranchId);
        }

        public override async Task<ActionResult<T>> Get(Guid id)
        {
            var ent = await _dbSet.AsNoTracking().Where(c => c.BranchId == BranchId).FirstOrDefaultAsync(c => c.Id == id);

            if (ent == null)
            {
                return NotFound();
            }

            return ent;
        }

        public override async Task<IActionResult> Put(Guid id, T ent)
        {
            ent.BranchId = BranchId;
            return await base.Put(id, ent);
        }

        public override async Task<ActionResult<T>> Post(T ent)
        {
            ent.BranchId = BranchId;
            return await base.Post(ent);
        }

        protected Guid BranchId
        {
            get
            {
                if (User.IsInRole("Super"))
                {
                    return new Guid(Request.Headers["branch"]);
                }

                return new Guid(User.Claims.Single(c => c.Type == "branch").Value);
            }
        }
    }
}
