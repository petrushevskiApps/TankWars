

public class AmmoPack : Pickable
{
    protected override void Collected(ICollector collector)
    {
        collector.PickableCollected(this);
        base.Collected(collector);
        
    }
}
