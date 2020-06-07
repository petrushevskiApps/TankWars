

public class AmmoPack : Pickable
{
    protected override void Collect(ICollector collector)
    {
        collector.PickableCollected(this);
        base.Collect(collector);
    }
}
