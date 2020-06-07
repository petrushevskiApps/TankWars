/*
 * IComparator uses Strategy Pattern to change
 * comparing alogrithm.
 */
public interface IComparator
{
    int CompareDetected(Detected d1, Detected d2);
}
