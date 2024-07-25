using BikeStore.Claims;
using BikeStore.Data;
using BikeStore.Models;
using BikeStore.Models.DTOs;
using BikeStore.Models.Options;
using BikeStore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BikeStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly BikeStoresContext _context;
        private readonly ISalesRepository _salesRepository;
        private readonly UserOptions _options;
        private readonly UserOptions _optionsSnapshot;

        public StoresController(
            BikeStoresContext context,
            ISalesRepository salesRepository,
            IOptions<UserOptions> options,
            IOptionsSnapshot<UserOptions> optionsSnapshot)
        {
            _context = context;
            _salesRepository = salesRepository;
            _options = options.Value;
            _optionsSnapshot = optionsSnapshot.Value;
        }

        [HttpGet]
        //[Authorize]
        //[RequiresClaim(StoresClaims.Read, "true")]
        public async Task<IActionResult> GetStores() {
            throw new NotImplementedException("Method not implemented");
            var stores = await _context.Stores.ToListAsync();

            return Ok(stores);
        }

        [HttpPost("AddStaffMember")]
        public async Task<ActionResult<Staff>> AddStaffMemberToStore(AddStaffDTO addStaffDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var managerId = await _context.Staffs
                                    .Where(staffMember =>
                                        staffMember.FirstName.Equals(addStaffDTO.ManagerName) ||
                                        staffMember.LastName.Equals(addStaffDTO.ManagerName))
                                    .Select(staffMember => staffMember.StaffId)
                                    .SingleAsync();

                var storeId = await _context.Stores
                                        .Where(store => store.StoreName.Equals(addStaffDTO.StoreName))
                                        .Select(store => store.StoreId)
                                        .SingleAsync();

                Staff staff = new Staff
                {
                    FirstName = addStaffDTO.FirstName,
                    LastName = addStaffDTO.LastName,
                    Email = addStaffDTO.Email,
                    Phone = addStaffDTO.Phone,
                    Active = addStaffDTO.Active,
                    StoreId = storeId,
                    ManagerId = managerId,
                };

                await _salesRepository.AddStaffMemberAsync(staff);

                return CreatedAtAction("AddStaffMemberToStore", new { Id = staff.StaffId }, staff);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest($"Parameter {e.ParamName} is not valid");
            }
        }


        [HttpPut("UpdateStaffMember/{id}")]
        public async Task<ActionResult<Staff>> UpdateStaffMember(int id, UpdateStaffDTO updatedStaffMemberDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Staff? staffMember = await _salesRepository.FindStaffMemberAsync(id);

                if(staffMember is null)
                {
                    return NotFound();
                }

                staffMember.FirstName = updatedStaffMemberDTO.FirstName;
                staffMember.LastName = updatedStaffMemberDTO.LastName;
                staffMember.Phone = updatedStaffMemberDTO.Phone;
                staffMember.Active = updatedStaffMemberDTO.Active;
                staffMember.Email = updatedStaffMemberDTO.Email;
                staffMember.StoreId = updatedStaffMemberDTO.Store.StoreId;
                staffMember.ManagerId = updatedStaffMemberDTO.Manager?.StaffId;

                await _salesRepository.UpdateStaffMemberAsync(staffMember);

                return NoContent();
            }
            catch (ArgumentNullException e)
            {
                return BadRequest($"Parameter {e.ParamName} is not valid");
            }
        }

        [HttpGet("options/{id}")]
        public IActionResult Options(int id)
        {
            return Ok(new
            {
                CountryOptions = _options.Country,
                IPAddressOptions = _options.IPAddress,
                CountryOptionsSnapshot = _optionsSnapshot.Country,
                IPAddressSnapshot = _optionsSnapshot.IPAddress
            });
        }

    }
}
