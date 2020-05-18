
public class HealthPack : Pickable
{
    public float amountToCollect = 0;

    protected override void Collected(ICollector collector)
    {
        base.Collected(collector);
        collector.PickableCollected(this);
    }
}
