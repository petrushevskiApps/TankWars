
public class StateKeys
{
    public const string ENEMY_DETECTED = "enemyDetected";
    public const string UNDER_ATTACK = "isUnderAttack";

    public const string HEALTH_AVAILABLE = "healthCount";
    public const string HEALTH_DETECTED = "healthDetected";

    public const string AMMO_AVAILABLE = "ammoAvailable";
    public const string AMMO_DETECTED = "ammoDetected";
    
    public const string HIDING_SPOT_DETECTED = "hiddingSpotDetected";

    public const string PATROL = "patrol";
}

public class FSMKeys
{
    public const string IDLE_STATE = "IdleState";
    public const string MOVETO_STATE = "MoveToState";
    public const string PERFORM_STATE = "PerformActionState";
}