using Boo.Lang;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventorySystem : MonoBehaviour, ICollector
{
    [SerializeField] private InventoryElement health;
    [SerializeField] private InventoryElement ammo;
	[SerializeField] private InventoryTime speedBoost;

	public InventoryElement Health { get => health; }
    
    public InventoryElement Ammo { get => ammo; }

	public InventoryTime SpeedBoost { get => speedBoost; }

	public void PickableCollected(AmmoPack collected)
	{
		Ammo.Increase(collected.amountToCollect);
	}

	public void PickableCollected(HealthPack collected)
	{
		Health.Increase(collected.amountToCollect);
	}
}
