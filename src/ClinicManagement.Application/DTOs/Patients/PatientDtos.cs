using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.DTOs.Patients
{
     class PatientDtos
    {

        public record PatientSignupRequest
        {
            public string NationalCode { get; init; }
            public string Name {  get; init; }
            public string Phone {  get; init; }

            public string Password {  get; init; }




            public PatientSignupRequest(
                
                string? nationalcode,
                string? name, 
                string? phone,
                string? password

                )
            {

                NationalCode=nationalcode?.Trim() ?? string.Empty;
                Name=name?.Trim() ?? string.Empty;
                Phone=phone?.Trim() ?? string.Empty;


            }


        }

     public record GetPatientByNationalCodeRequest
      {

          public string NationalCode { get; init; }



           public GetPatientByNationalCodeRequest(

             string? nationalcode)
          {
              NationalCode=nationalcode?.Trim() ?? string.Empty;

          }
             


      }

       public record GetPatientsRequest
      {




        }

        public record UpdatePatientRequest
        {

            public string NationalCode { get; init; }
            public string Name { get; init; }
            public string Phone { get; init; }






            public UpdatePatientRequest(

                string? nationalcode,
                string? name,
                string? phone

                )
            {
                NationalCode = nationalcode?.Trim() ?? string.Empty;
                Name = name?.Trim() ?? string.Empty;
                Phone = phone?.Trim() ?? string.Empty;

            }

        }


        public record DeletePatientRequest
        {

            public string NationalCode { get; init; }



            public DeletePatientRequest(

                string? nationalcode
                )
            {
                NationalCode = nationalcode ?? string.Empty;

            }


        }

//        public record PatientResponse
//        {
//            public string NationalCode { get; init; }
//            public string Name { get; init; }
//            public string Phone { get; init; }


//            public PatientResponse(

//                string? nationalcode,
//                string? name,
//                string? phone
//                )
//            {

//                NationalCode=nationalcode? .Trim() ?? string.Empty;
//                Name=name?.Trim() ?? string.Empty;
//                Phone=phone?.Trim() ?? string.Empty;
//            }

//        }
//    }
//}










