namespace TheBoringChat.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<SignUpRequest, Users>()
            .ForMember(dest => dest.Password, otp => otp.MapFrom(src => src.Password.ToBCryptHash()))
            .ForMember(dest => dest.CreatedAt, otp => otp.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.Status, otp => otp.MapFrom(_ => 1));

        CreateMap<Users, UserDto>();
        CreateMap<UserDto, Users>();
        CreateMap<UpdateUserRequest, UpdateUserDto>();
        CreateMap<ForgotPasswordRequest, UpdateUserDto>()
            .ForMember(dest => dest.NewPassword, otp => otp.MapFrom(src => src.NewPw.ToBCryptHash()));
    }
}
