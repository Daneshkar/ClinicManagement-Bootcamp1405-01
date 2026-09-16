using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Treatment;

namespace ClinicManagement.Application.Interfaces.Services
{
    public interface ITreatmentService
    {
        /// <summary>
        /// Retrieves today's appointment queue for a specific doctor, applying automated status updates.
        /// </summary>
        /// <param name="doctorMedicalId">The medical identifier of the doctor.</param>
        /// <param name="currentDate">The current date used to filter and evaluate today's appointments.</param>
        /// <returns>A <see cref="Result{T}"/> wrapping the collection of today's appointments.</returns>
        Task<Result<IEnumerable<TodayAppointmentResponse>>> GetTodayAppointmentsAsync(string doctorMedicalId, DateTime currentDate);

        /// <summary>
        /// Validates and records prescription details for a patient appointment.
        /// </summary>
        /// <param name="doctorMedicalId">The medical identifier of the doctor issuing the prescription.</param>
        /// <param name="request">The prescription details to register.</param>
        /// <param name="currentTime">The current time used for validation and record-keeping.</param>
        /// <returns>A <see cref="Result"/> indicating success or failure of the registration.</returns>
        Task<Result> RegisterPrescriptionAsync(string doctorMedicalId, RegisterPrescriptionRequest request, DateTime currentTime);
    }
}