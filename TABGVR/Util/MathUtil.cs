namespace TABGVR.Util;

public static class MathUtil
{ 
    public static int CanonicalMod(int a, int b)
    {
        return ((a % b) + b) % b;
    }
}