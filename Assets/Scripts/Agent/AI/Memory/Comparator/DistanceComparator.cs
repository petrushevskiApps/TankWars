/*
 * Strategy Pattern implementation for
 * comparing detecteds according to distance
 */
public class DistanceComparator : IComparator
{
    public int CompareDetected(Detected d1, Detected d2)
    {
        if (d1.IsValid() && d2.IsValid())
        {
            if (d1.GetDistance() > d2.GetDistance()) return 1;
            else if (d1.GetDistance() < d2.GetDistance()) return -1;
            else return 0;
        }
        else if (d1.IsValid() && !d2.IsValid())
        {
            return -1;
        }
        else if (!d1.IsValid() && d2.IsValid())
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
