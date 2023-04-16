using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Dtos.Apparel
{
    public class AddApparelDto
    {
        public string Name { get; set; } = string.Empty;
        public int Resistance { get; set; }
        public int CharacterId { get; set; }
    }
}