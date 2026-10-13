namespace Gameplay.Data
{
    public static class StatTypes
    {
        public static class Damage
        {
            public static readonly StatType Physical = new (BaseType.Damage, DamageType.Physical);
            public static readonly StatType Fire = new (BaseType.Damage, DamageType.Physical);
            public static readonly StatType Cold = new (BaseType.Damage, DamageType.Physical);
            public static readonly StatType Lightning = new (BaseType.Damage, DamageType.Physical);
        }
        
        public static class Resistance
        {
            public static readonly StatType Physical = new (BaseType.Resistance, DamageType.Physical);
            public static readonly StatType Fire = new (BaseType.Resistance, DamageType.Physical);
            public static readonly StatType Cold = new (BaseType.Resistance, DamageType.Physical);
            public static readonly StatType Lightning = new (BaseType.Resistance, DamageType.Physical);
        }
        
        public static class Speed
        {
            public static readonly StatType Attack = new (BaseType.Speed, ActionType.Attack);
            public static readonly StatType Spell = new (BaseType.Speed, ActionType.Cast);
            public static readonly StatType Move = new (BaseType.Speed, ActionType.Move);
        }
        
        public static class Resource
        {
            public static readonly StatType Life = new (BaseType.Resource, ResourceType.Life);
            public static readonly StatType Mana = new (BaseType.Resource, ResourceType.Mana);
            public static readonly StatType Stamina = new (BaseType.Resource, ResourceType.Stamina);
            public static readonly StatType Ammo = new (BaseType.Resource, ResourceType.Ammo);
        }
        
        public static class ResourceRecovery
        {
            public static readonly StatType Life = new (BaseType.ResourceRecovery, ResourceType.Life);
            public static readonly StatType Mana = new (BaseType.ResourceRecovery, ResourceType.Mana);
            public static readonly StatType Stamina = new (BaseType.ResourceRecovery, ResourceType.Stamina);
            public static readonly StatType Ammo = new (BaseType.ResourceRecovery, ResourceType.Ammo);
        }
        
        public static class Defence
        {
            public static readonly StatType Armor = new (BaseType.Defence, DefenceType.Armor);
            public static readonly StatType Evasion = new (BaseType.Defence, DefenceType.Evasion);
            public static readonly StatType Barrier = new (BaseType.Defence, DefenceType.Barrier);
        }
        
        public static class Skill
        {
            public static readonly StatType Repeat = new (BaseType.Skill, SkillStat.Repeat);
            public static readonly StatType Targets = new (BaseType.Skill, SkillStat.Targets);
            public static readonly StatType Chain = new (BaseType.Skill, SkillStat.Chain);
            public static readonly StatType Area = new (BaseType.Skill, SkillStat.Area);
        }
        
        public static class Projectile
        {
            public static readonly StatType Count = new (BaseType.Projectile, ProjectileStat.Count);
            public static readonly StatType Speed = new (BaseType.Projectile, ProjectileStat.Speed);
            public static readonly StatType Range = new (BaseType.Projectile, ProjectileStat.Range);
            public static readonly StatType Pierce = new (BaseType.Projectile, ProjectileStat.Pierce);
        }
        
        public static class Barrier
        {
            public static readonly StatType StartTime = new (BaseType.Barrier, BarrierStat.StartTime);
            public static readonly StatType RegenRate = new (BaseType.Barrier, BarrierStat.RegenRate);
        }
        
        public static class Penetration
        {
            public static readonly StatType Physical = new (BaseType.Penetration, DamageType.Physical);
            public static readonly StatType Fire = new (BaseType.Penetration, DamageType.Physical);
            public static readonly StatType Cold = new (BaseType.Penetration, DamageType.Physical);
            public static readonly StatType Lightning = new (BaseType.Penetration, DamageType.Physical);
        }
    }
}