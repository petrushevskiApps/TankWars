using UnityEngine;

/*
 * Strategy Pattern implementation for
 * Detected of type Enemy ( Context )
 * Comparing by Health and Distance
 */
public class EnemyComparator : IComparator
{
    public float healthDifferenceThreshold = 10f;

    public int CompareDetected(Detected d1, Detected d2)
    {
        if (d1.IsValid() && d2.IsValid())
        {
            float d2Health = d2.GetAgentHealth();
            float d1Health = d1.GetAgentHealth();

            if (Mathf.Abs(d1Health - d2Health) > healthDifferenceThreshold)
            {
                return d1Health < d2Health ? -1 : 1;
            }
            else
            {
                float d1Distance = d1.GetDistance();
                float d2Distance = d2.GetDistance();

                if (d1Distance < d2Distance)
                {
                    return -1;
                }
                else if (d1Distance > d2Distance)
                {
                    return 1;
                }
                else return 0;
            }

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
