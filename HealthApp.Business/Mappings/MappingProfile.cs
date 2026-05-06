// HealthApp.Business/Mappings/MappingProfile.cs

using AutoMapper;
using HealthApp.Business.DTOs;
using HealthApp.Domain.Entities;
using HealthApp.Domain.Enums;
using System.Linq;

namespace HealthApp.Business.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateChildDto, Child>();

            CreateMap<Child, ChildDto>()
                .ForMember(dest => dest.VaccineSchedule, opt => opt.MapFrom(src => src.VaccineSchedules))
                .ForMember(dest => dest.Vaccines, opt => opt.MapFrom(src => src.Vaccines))
                .ReverseMap();

            CreateMap<Vaccine, VaccineDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString().ToLower()))
                .ReverseMap();

            CreateMap<VaccineSchedule, VaccineScheduleDto>()
                .ForCtorParam("Id", opt => opt.MapFrom(src => src.Id))
                .ForCtorParam("Period", opt => opt.MapFrom(src => src.Period))
                .ForCtorParam("MonthIndex", opt => opt.MapFrom(src => src.MonthIndex))
                .ForCtorParam("ScheduledDate", opt => opt.MapFrom(src => src.ScheduledDate))
                .ForCtorParam("Vaccines", opt => opt.MapFrom(src => src.Vaccines))
                .ForCtorParam("IsCompleted", opt => opt.MapFrom(src => src.Vaccines.All(v => v.Status == VaccineStatus.Completed)))
                .ForCtorParam("CompletedCount", opt => opt.MapFrom(src => src.Vaccines.Count(v => v.Status == VaccineStatus.Completed)))
                .ForCtorParam("PendingCount", opt => opt.MapFrom(src => src.Vaccines.Count(v => v.Status == VaccineStatus.Pending)));

            CreateMap<User, UserDto>().ReverseMap();

            // YENİ EKLENDİ: Reminder eşlemesi
            // Eğer ReminderDto kullanmıyorsan direkt entity üzerinden devam edebiliriz.
            CreateMap<Reminder, Reminder>().ReverseMap();
        }
    }
}