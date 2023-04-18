using System.Text.Json.Serialization;

namespace dotnet_rpg.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RpgClass
    {
        Knight = 1,
        Mage = 2,
        Cleric = 3,
        Ranger = 4,
        Support = 5,
        Scout = 6,
        Archer = 7,
        Warrior = 8,
        Paladin = 9,
        Vanguard = 10,

        Ringbearer = 11

    }
}