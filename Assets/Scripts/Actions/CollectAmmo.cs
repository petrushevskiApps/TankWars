using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectAmmo : GoapAction 
{
	private Vector3 destination;

	bool completed = false;

	public CollectAmmo() 
	{
		name = "CollectAmmo";

		AddPrecondition(StateKeys.AMMO_AMOUNT, false);
		AddPrecondition(StateKeys.AMMO_DETECTED, true);

		AddEffect(StateKeys.AMMO_AMOUNT, true);
	}
	
	public override void Reset ()
	{
		destination = transform.position;
		completed = false;
	}
	
	public override bool IsActionDone ()
	{
		return completed;
	}
	
	public override bool RequiresInRange ()
	{
		target = GetComponent<Tank>().agentMemory.GetAmmoLocation();
		return true; 
	}
	
	public override bool CheckProceduralPrecondition (GameObject agent)
	{	
		return true;
	}
	
	public override void Perform(GameObject agent, Action success, Action fail)
	{
		Debug.Log($"<color=green> {gameObject.name} Perform Action: {this.name}</color>");
		StartCoroutine(Collect(agent, success, fail));
	}

	IEnumerator Collect(GameObject agent, Action succes, Action fail)
	{
		yield return new WaitForSeconds(2f);

		if (GetComponent<Tank>().agentMemory.IsAmmoDetected())
		{
			GetComponent<Tank>().agentMemory.AddAmmo();
			GetComponent<Tank>().agentMemory.RemoveDetectedAmmo(target);
			succes.Invoke();
			completed = true;
		}
		else
		{
			fail.Invoke();
		}
	}

	
}
