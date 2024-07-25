namespace BikeStore.Models.DTOs;

public record UpdateStaffDTO(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    byte Active,
    Store Store,
    Staff? Manager
    );