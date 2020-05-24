

public class AmmoPack : Pickable
{
    public int amountToCollect = 0;

    public override void Collect(ICollector collector)
    {
        collector.PickableCollected(this);
        base.Collect(collector);
    }
}
