using AutoMapper;
using PasswordWallet.Server.Entities;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Server.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Account, AccountDto>();
        CreateMap<AccountDto, Account>();
        CreateMap<Credential, CredentialDto>();
        CreateMap<CredentialDto, Credential>();
        CreateMap<LoginIpAddress, LoginIpAddressDto>();
    }
}