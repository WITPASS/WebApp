using System;
using System.Threading.Tasks;

namespace Api
{
    public partial class SeedData
    {
        public static async Task LocalData(AppDbContext context, Guid adminId, Guid adminsRoleId)
        {
            await Task.CompletedTask;
        }
    }
}