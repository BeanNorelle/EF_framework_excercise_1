namespace dotnet_rpg.Services.WeaponService
{
    public interface IWeaponService
    {
        public Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
    }
}