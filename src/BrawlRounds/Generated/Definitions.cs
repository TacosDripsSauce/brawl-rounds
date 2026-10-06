namespace BrawlRounds.Generated
{
    internal abstract class WeaponDefinition
    {
        protected WeaponDefinition(string id, string displayName, string kind, float damagePercent, float baseKnockback, float range, float cooldown, int projectiles, float spread, float moveSpeedMultiplier, float pickupWeight)
        { Id=id; DisplayName=displayName; Kind=kind; DamagePercent=damagePercent; BaseKnockback=baseKnockback; Range=range; Cooldown=cooldown; Projectiles=projectiles; Spread=spread; MoveSpeedMultiplier=moveSpeedMultiplier; PickupWeight=pickupWeight; }
        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string Kind { get; private set; }
        public float DamagePercent { get; private set; }
        public float BaseKnockback { get; private set; }
        public float Range { get; private set; }
        public float Cooldown { get; private set; }
        public int Projectiles { get; private set; }
        public float Spread { get; private set; }
        public float MoveSpeedMultiplier { get; private set; }
        public float PickupWeight { get; private set; }
    }
    internal abstract class SystemDefinition
    {
        protected SystemDefinition(string id,string trigger,string implementation,string[] dependsOn){Id=id;Trigger=trigger;Implementation=implementation;DependsOn=dependsOn;}
        public string Id {get;private set;} public string Trigger{get;private set;} public string Implementation{get;private set;} public string[] DependsOn{get;private set;}
    }
    internal abstract class HookDefinition
    {
        protected HookDefinition(string id,string target,string method,string hookType,string system){Id=id;Target=target;Method=method;HookType=hookType;System=system;}
        public string Id{get;private set;} public string Target{get;private set;} public string Method{get;private set;} public string HookType{get;private set;} public string System{get;private set;}
    }
}
