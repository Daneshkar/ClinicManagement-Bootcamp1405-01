using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }
    public string DoctorMedicalId { get; private set; }
    public string PatientNationalCode { get; private set; }

    // Encapsulated properties
    public DateTime VisitDate { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string? Prescription { get; private set; }

    // Navigation properties
    public Doctor? Doctor { get; private set; }
    public Patient? Patient { get; private set; }

    // Private constructor for EF Core materialization & preventing direct instantiation
    private Appointment() { }

    // Static Factory Method
    public static Appointment Create(
        string doctorMedicalId,
        string patientNationalCode,
        DateTime visitDate,
        AppointmentStatus status = AppointmentStatus.Reserved)
    {
        return new Appointment
        {
            Id = Guid.NewGuid(),
            DoctorMedicalId = doctorMedicalId,
            PatientNationalCode = patientNationalCode,
            VisitDate = visitDate,
            Status =status,
            Prescription = null
        };
    }
}