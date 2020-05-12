
public class HealthPack : Pickable
{
    protected override void Collected(ICollector collector)
    {
        base.Collected(collector);
        collector.PickableCollected(this);
    }
}
