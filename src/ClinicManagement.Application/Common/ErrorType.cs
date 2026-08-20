namespace ClinicManagement.Application.Common;

public enum ErrorType
{
    None = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Failure = 4,
    Unauthorized = 5, // 401 Unauthorized
    Forbidden = 6     // 403 Forbidden
}