using BikeStore.Data;
using BikeStore.Models;
using BikeStore.Models.DTOs;

namespace BikeStore.Repositories
{
    public class SalesRepository : ISalesRepository
    {
        private readonly BikeStoresContext _context;

        public SalesRepository(BikeStoresContext context)
        {
            _context = context;
        }

        public async Task AddStaffMemberAsync(Staff staff)
        {
            await _context.Staffs.AddAsync(staff);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateStaffMemberAsync(Staff updatedStaffMember)
        {
            var staffUpdated = _context.Staffs.Update(updatedStaffMember);

            await _context.SaveChangesAsync();
        }

        public async Task<Staff?> FindStaffMemberAsync(int id)
        {
            return await _context.Staffs.FindAsync(id);
        }
    }
}
