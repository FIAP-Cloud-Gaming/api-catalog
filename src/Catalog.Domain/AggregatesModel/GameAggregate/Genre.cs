namespace Catalog.Domain.AggregatesModel.GameAggregate;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Genre
{
    Action = 1,
    Adventure = 2,
    RPG = 3,
    Strategy = 4,
    Sports = 5,
    Racing = 6,
    Shooter = 7,
    Fighting = 8,
    Platformer = 9,
    Puzzle = 10,
    Horror = 11,
    Simulation = 12,
    MMORPG = 13,
    MOBA = 14,
    BattleRoyale = 15,
    Survival = 16,
    Sandbox = 17,
    Stealth = 18,
    RhythmMusic = 19,
    Educational = 20
}
