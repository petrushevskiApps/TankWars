
public class StateKeys
{
    public const string ENEMY_DETECTED = "enemyDetected";
    public const string UNDER_ATTACK = "isUnderAttack";

    public const string HEALTH_FULL = "healthFull";
    public const string HEALTH_DETECTED = "healthDetected";

    public const string AMMO_FULL = "ammoFull";
    public const string AMMO_DETECTED = "ammoDetected";
    
    public const string HIDING_SPOT_DETECTED = "hiddingSpotDetected";

    public const string ENEMY_KILLED = "enemyKilled";
}

public class FSMKeys
{
    public const string IDLE_STATE = "IdleState";
    public const string MOVETO_STATE = "MoveToState";
    public const string PERFORM_STATE = "PerformActionState";
}