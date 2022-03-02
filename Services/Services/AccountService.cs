using AutoMapper;
using DAL.Entities;
using DAL.Entities.Identity.Enums;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Models.Common;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly IGenericRepository<City> _cityRepository;
        private readonly IMapper _mapper;
        public AccountService(IGenericRepository<City> cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        public List<OptionDto> GetAccountStates()
            => Enum.GetValues<AccountState>().Cast<AccountState>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();

        public async Task<IReadOnlyList<OptionDto>> GetCities()
            => _mapper.Map<IReadOnlyList<City>, IReadOnlyList<OptionDto>>(await _cityRepository.GetAllAsync());

        public List<OptionDto> GetGenders()
            => Enum.GetValues<Gender>().Cast<Gender>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();

        public List<OptionDto> GetRoles()
            => Enum.GetValues<Roles>().Cast<Roles>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();

        public List<OptionDto> GetUserTypes()
            => Enum.GetValues<UserType>().Cast<UserType>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();

    }
    public interface IAccountService
    {
        public Task<IReadOnlyList<OptionDto>> GetCities();
        public List<OptionDto> GetGenders();
        public List<OptionDto> GetUserTypes();
        public List<OptionDto> GetAccountStates();
        public List<OptionDto> GetRoles();

    }
}