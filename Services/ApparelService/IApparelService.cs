using dotnet_rpg.Dtos.Apparel;

namespace dotnet_rpg.Services.ApparelService
{
    public interface IApparelService
    {
        public Task<ServiceResponse<GetCharacterDto>> AddApparel(AddApparelDto newApparel);
    }
}