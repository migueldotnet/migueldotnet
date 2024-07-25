namespace BikeStore.Models.DTOs;

public record AddStaffDTO(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    byte Active,
    string StoreName,
    string ManagerName
    );
    
