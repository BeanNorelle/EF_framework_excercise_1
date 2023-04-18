using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using dotnet_rpg.Dtos.Fight;

namespace dotnet_rpg.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        public IMapper _mapper { get; }

        public FightService(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }



        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                var Attacker = await _context.Characters
                            .Include(c => c.Weapon)
                            .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                var Opponent = await _context.Characters
                        .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                if (Attacker is null || Opponent is null || Attacker.Weapon is null || Opponent.Apparel is null)
                {
                    throw new Exception("Something wrong with the attack...");
                }

                int damage = DoWeaponAttack(Attacker, Opponent);

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = Attacker.Name,
                    Opponent = Opponent.Name,
                    OpponentHP = Opponent.HitPoints,
                    AttackerHP = Attacker.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static int DoWeaponAttack(Character Attacker, Character Opponent)
        {
            if (Attacker.Weapon is null)
            {
                throw new Exception($"{Attacker.Name} has no weapons!");
            }

            var Apparel = (Opponent.Apparel is null) ? 0 : Opponent.Apparel.Resistance;

            int damage = Attacker.Weapon.Damage + (new Random().Next(Attacker.Strength));
            damage -= Apparel + (new Random().Next(Opponent.Defense));

            if (damage > 0) Opponent.HitPoints -= damage;

            return damage;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                var Attacker = await _context.Characters
                            .Include(c => c.Skills)
                            .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                var Opponent = await _context.Characters
                          .FirstOrDefaultAsync(c => c.Id == request.OpponentId);


                if (Attacker is null || Opponent is null || Attacker.Skills is null)
                {
                    throw new Exception("Something wrong with the attack...");
                }


                if (Attacker.Skills is null)
                {
                    throw new Exception("Something wrong with the skills");
                }

                var Skill = Attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);
                if (Skill is null)
                {
                    response.Success = false;
                    response.Message = $"{Attacker.Name} doesn't know that skill!";
                    return response;
                }

                var Apparel = (Opponent.Apparel is null) ? 0 : Opponent.Apparel.Resistance;
                int damage = DoSkillAttack(Attacker, Opponent, Skill, Apparel);
                if (Opponent.HitPoints <= 0) response.Message = $"{Opponent.Name} has been defeated!";

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = Attacker.Name,
                    Opponent = Opponent.Name,
                    OpponentHP = Opponent.HitPoints,
                    AttackerHP = Attacker.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static int DoSkillAttack(Character Attacker, Character Opponent, Skill Skill, int Apparel)
        {
            int damage = Skill.Damage + (new Random().Next(Attacker.Intelligence));
            damage -= Apparel + (new Random().Next(Opponent.Defense));

            if (damage > 0)
                Opponent.HitPoints -= damage;

            return damage;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            var response = new ServiceResponse<FightResultDto>
            {
                Data = new FightResultDto()
            };

            try
            {
                var characters = await _context.Characters
                                .Include(c => c.Weapon)
                                .Include(c => c.Skills)
                                .Where(c => request.CharacterIds.Contains(c.Id))
                                .ToListAsync();

                bool defeated = false;

                while (!defeated)
                {
                    foreach (var attacker in characters)
                    {
                        var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        var opponent = opponents[new Random().Next(opponents.Count)];
                        var apparel = (opponent.Apparel is null) ? 0 : opponent.Apparel.Resistance;

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon && attacker.Weapon is not null)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);
                        }
                        else if (!useWeapon && attacker.Skills is not null)
                        {
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            damage = DoSkillAttack(attacker, opponent, skill, apparel);
                        }
                        else
                        {
                            response.Data.Log.Add($"{attacker.Name} wasn't able to attack!");
                            continue;
                        }

                        response.Data.Log
                                .Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage");

                        if (opponent.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated!");
                            response.Data.Log.Add($"{attacker.Name} wind with {attacker.HitPoints} HP left!");
                            break;
                        }
                    }
                }

                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HitPoints = 100;
                });

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScore()
        {
            var characters = await _context.Characters.Where(c => c.Fights > 0)
                                                    .OrderByDescending(c => c.Victories)
                                                    .ThenBy(c => c.Defeats).ToListAsync();

            var response = new ServiceResponse<List<HighScoreDto>>()
            {
                Data = characters.Select(c => _mapper.Map<HighScoreDto>(c)).ToList()
            };

            return response;
        }
    }
}