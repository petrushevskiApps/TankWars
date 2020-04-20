using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : GoapAction 
{

	bool completed = false;
	float startTime = 0;
	public float workDuration = 2; // seconds
	
	public bool isDeliveryReady = false;


	public Patrol() 
	{
		AddEffect("patrol", true);
		name = "Patrol";
		//target = CornerCalculator.Instance.GetRandomInWorldCoordinates();
	}
	
	public override void Reset ()
	{
		
		completed = false;
		startTime = 0;
	}
	
	public override bool IsActionDone ()
	{
		return completed;
	}
	
	public override bool RequiresInRange ()
	{
		return true; 
	}
	
	public override bool CheckProceduralPrecondition (GameObject agent)
	{	
		return true;
	}
	
	public override bool Perform (GameObject agent)
	{
		target = CornerCalculator.Instance.GetRandomInWorldCoordinates();
		completed = true;
		return true;
	}
	
	
}
