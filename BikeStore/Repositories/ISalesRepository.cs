using BikeStore.Models;
using BikeStore.Models.DTOs;

namespace BikeStore.Repositories;

public interface ISalesRepository
{
    Task AddStaffMemberAsync(Staff staff);
    Task UpdateStaffMemberAsync(Staff updatedStaffMember);
    Task<Staff?> FindStaffMemberAsync(int id);
}
