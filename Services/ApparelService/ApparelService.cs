
using System.Security.Claims;
using dotnet_rpg.Dtos.Apparel;
using dotnet_rpg.Migrations;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.ApparelService
{
    public class ApparelService : IApparelService
    {


        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ApparelService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<ServiceResponse<GetCharacterDto>> AddApparel(AddApparelDto newApparel)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == newApparel.CharacterId &&
                c.User!.Id == int.Parse(_httpContextAccessor.HttpContext!.User
                .FindFirstValue(ClaimTypes.NameIdentifier)!));

                if (character is null)
                {
                    response.Success = false;
                    response.Message = "Character not found";
                    return response;
                }

                var apparel = new Apparel
                {
                    Name = newApparel.Name,
                    Resistance = newApparel.Resistance,
                    Character = character
                };

                _context.Apparels.Add(apparel);
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}