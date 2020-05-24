
public class StateKeys
{
    public const string ENEMY_DETECTED = "enemyDetected";
    public const string IN_SHOOTING_RANGE = "inShootingRange";
    public const string UNDER_ATTACK = "isUnderAttack";
    public const string HEALTH_AMOUNT = "healthCount";
    public const string AMMO_AVAILABLE = "ammoAvailable";
    public const string AMMO_DETECTED = "ammoDetected";
    public const string AMMO_COLLECT = "ammoCollect";
    public const string AMMO_SPECIAL_AMOUNT = "specialAmmoAmount";
    public const string HEALTH_DETECTED = "healthDetected";
    public const string HIDING_SPOT_DETECTED = "hiddingSpotDetected";
    public const string HIDING = "hiding";
    public const string PATROL = "patrol";
}

public class GoalKeys
{
    public const string GET_READY = "getReady";
    
    public const string ELIMINATE_ENEMY = "eliminateEnemy";
    public const string SURVIVE = "survive";
}

public class FSMKeys
{
    public const string IDLE_STATE = "IdleState";
    public const string MOVETO_STATE = "MoveToState";
    public const string PERFORM_STATE = "PerformActionState";
}