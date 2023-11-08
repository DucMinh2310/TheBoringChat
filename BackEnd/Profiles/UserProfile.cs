namespace BackEnd.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<SignUpRequest, Users>()
            .ForMember(dest => dest.Password, otp => otp.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)))
            .ForMember(dest => dest.CreatedAt, otp => otp.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.Status, otp => otp.MapFrom(_ => 1));
    }
}
