
public class HealthPack : Pickable
{
    public float amountToCollect = 0;

    public override void Collect(ICollector collector)
    {
        collector.PickableCollected(this);
        base.Collect(collector);
    }
}
