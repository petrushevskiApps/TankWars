using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectAmmo : GoapAction 
{
	private Vector3 destination;

	bool completed = false;
	private Memory agentMemory;

	public CollectAmmo() 
	{
		name = "CollectAmmo";

		AddPrecondition(StateKeys.AMMO_AMOUNT, false);
		AddPrecondition(StateKeys.AMMO_DETECTED, true);

		AddEffect(StateKeys.AMMO_AMOUNT, true);

	}
	private void Start()
	{
		agentMemory = GetComponent<Tank>().agentMemory;
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
		target = agentMemory.AmmoPacks.GetDetected();
		return true; 
	}
	
	public override bool CheckProceduralPrecondition (GameObject agent)
	{	
		return true;
	}
	
	public override void Perform(GameObject agent, Action success, Action fail)
	{
		Debug.Log($"<color=green> {gameObject.name} Perform Action: {this.name}</color>");
		StartCoroutine(Collect(success, fail));
	}

	IEnumerator Collect(Action succes, Action fail)
	{
		yield return new WaitForSeconds(2f);

		if (ammoCollected)
		{
			succes.Invoke();
			completed = true;
		}
		else
		{
			agentMemory.AmmoPacks.RemoveDetected(target);
			fail.Invoke();
		}
	}

	private bool ammoCollected = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Pickable"))
		{
			if (!ammoCollected)
			{
				agentMemory.IncreaseAmmo();
				agentMemory.AmmoPacks.RemoveDetected(target);
				ammoCollected = true;
			}
		}
	}
	

}
