

public class AmmoPack : Pickable
{
    public int amountToCollect = 0;

    protected override void Collected(ICollector collector)
    {
        collector.PickableCollected(this);
        base.Collected(collector);
        
    }
}
