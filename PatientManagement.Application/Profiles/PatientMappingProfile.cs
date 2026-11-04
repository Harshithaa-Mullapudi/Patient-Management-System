using AutoMapper;
using PatientManagement.Application.DTOs;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Profiles;

public class PatientMappingProfile : Profile
{
    public PatientMappingProfile()
    {
        // Patient to Response DTO
        CreateMap<Patient, PatientResponseDto>()
            .ForMember(
                destination => destination.FullName,
                options => options.MapFrom(source =>
                    source.FirstName + " " + source.LastName)
            )
            .ReverseMap();

        // Create DTO to Patient entity
        CreateMap<CreatePatientDto, Patient>()
            .ForCtorParam(
                "dateOfBirth",
                options => options.MapFrom(source => source.DateOfBirth!.Value)
            );

        CreateMap<CreatePatientProfileDto, PatientProfile>();
        CreateMap<PatientProfile, PatientProfileResponseDto>();

        CreateMap<CreateAppointmentDto, Appointment>();
        CreateMap<Appointment, AppointmentResponseDto>();
    }
} 