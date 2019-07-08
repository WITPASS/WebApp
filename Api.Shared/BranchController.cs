using Data;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Shared
{
    public class BranchEntityController<T, T2> : EntityController<T, T2> where T : DbContext where T2 : BranchEntity
    {
        public BranchEntityController(T context, DbSet<T2> dbSet) : base(context, dbSet) { }

        public override IEnumerable<T2> Get()
        {
            return _dbSet.AsNoTracking().Where(c => c.BranchId == BranchId);
        }

        public override SingleResult<T2> Get(Guid id)
        {
            var result = _dbSet.AsNoTracking().Where(c => c.BranchId == BranchId && c.Id == id);
            return SingleResult.Create(result);
        }

        public override async Task<IActionResult> Put(Guid id, T2 ent)
        {
            ent.BranchId = BranchId;
            return await base.Put(id, ent);
        }

        public override async Task<ActionResult<T2>> Post(T2 ent)
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
                    return new Guid(Request.Headers["branchid"]);
                }

                return new Guid(User.Claims.Single(c => c.Type == "branchid").Value);
            }
        }
    }
}
