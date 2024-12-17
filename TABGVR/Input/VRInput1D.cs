using System;

namespace TABGVR.Input;

public class VRInput1D
{
    public float X = 0;

    public int Digital => (Math.Abs(X) > 0.8) ? (X > 0 ? 1 : -1) : 0;
    public int LastFrameDigital = 0;
}