using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupFlour : GoapAction 
{

	bool completed = false;
	float startTime = 0;
	public float workDuration = 2; // seconds
	public Inventory windmill;
	
	public PickupFlour () 
	{
		AddPrecondition("hasStock", true);
		AddPrecondition ("hasFlour", false); 
		AddEffect ("hasFlour", true);
		name = "PickupFlour";
	}
	
	public override void reset ()
	{
		completed = false;
		startTime = 0;
	}
	
	public override bool isDone ()
	{
		return completed;
	}
	
	public override bool RequiresInRange ()
	{
		return true; 
	}
	
	public override bool CheckProceduralPrecondition (GameObject agent)
	{	
		return windmill.flourLevel >= 5;
	}
	
	public override bool Perform (GameObject agent)
	{
		if (startTime == 0)
		{
			Debug.Log("Starting: " + name);
			startTime = Time.time;
		}

		if (Time.time - startTime > workDuration) 
		{
			Debug.Log("Finished: " + name);
			this.GetComponent<Inventory>().flourLevel += 5;
			windmill.flourLevel -= 5;
			completed = true;
		}
		return true;
	}
	
}
