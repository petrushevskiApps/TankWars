using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverBread : GoapAction {

	bool completed = false;
	float startTime = 0;
	public float workDuration = 2; // seconds
	
	public DeliverBread () 
	{
		AddPrecondition ("hasDelivery", true); 
		AddEffect ("doJob", true);
		name = "DeliverBread";
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
		if (startTime == 0)
		{
			Debug.Log("Starting: " + name);
			startTime = Time.time;
		}

		if (Time.time - startTime > workDuration) 
		{
			Debug.Log("Finished: " + name);
			this.GetComponent<Inventory>().breadLevel -= this.GetComponent<Inventory>().breadLevel;
			completed = true;
		}
		return true;
	}
	
}
