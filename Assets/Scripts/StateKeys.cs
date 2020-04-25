using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StateKeys
{
    public const string ENEMY_DETECTED = "enemyDetected";
    public const string HEALTH_AMOUNT = "healthCount";
    public const string AMMO_AMOUNT = "ammoAvailable";
    public const string AMMO_DETECTED = "ammoDetected";
    public const string AMMO_COLLECT = "ammoCollect";
    public const string AMMO_SPECIAL_AMOUNT = "specialAmmoAmount";
}

public class GoalKeys
{
    public const string GET_READY = "getReady";
    public const string PATROL = "patrol";
    public const string ELIMINATE_ENEMY = "eliminateEnemy";
}

public class FSMKeys
{
    public const string IDLE_STATE = "IdleState";
    public const string MOVETO_STATE = "MoveToState";
    public const string PERFORM_STATE = "PerformActionState";
}