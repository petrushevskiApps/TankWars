using Boo.Lang;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventorySystem : MonoBehaviour, ICollector
{
    [SerializeField] private InventoryHealth health;
    [SerializeField] private InventoryAmmo ammo;

    public InventoryHealth Health { get => health; }
    
    public InventoryAmmo Ammo { get => ammo; }

	public void PickableCollected(AmmoPack collected)
	{
		Ammo.Increase(collected.amountToCollect);
	}

	public void PickableCollected(HealthPack collected)
	{
		Health.Increase(collected.amountToCollect);
	}


}
