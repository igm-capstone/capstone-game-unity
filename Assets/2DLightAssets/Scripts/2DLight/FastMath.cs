using UnityEngine;

public static class FastMath
{
    private const int Bits = 7;

    private const int Bits2 = Bits << 1;
    private const int Mask = ~(-1 << Bits2);
    private const int Count = Mask + 1;
    private static readonly int Dim = (int)Mathf.Sqrt(Count);

    private static readonly float InvDimMinus1 = 1.0f / (Dim - 1);

    private const float Deg = 180.0f / Mathf.PI;

    public  static float[] ATan2Array { get; private set; }

    public static float[] SinArray { get; private set; }

    public static float[] CosArray { get; private set; }

    static FastMath()
    {
        SinArray = new float[360];
        CosArray = new float[360];

        for (int i = 0; i < 360; i++)
        {
            SinArray[i] = Mathf.Sin(i * Mathf.Deg2Rad);
            CosArray[i] = Mathf.Cos(i * Mathf.Deg2Rad);
        }

        ATan2Array = new float[Count];
        for (var i = 0; i < Dim; i++)
        {
            for (var j = 0; j < Dim; j++)
            {
                var x0 = (float)i / Dim;
                var y0 = (float)j / Dim;

                ATan2Array[j * Dim + i] = (float)Mathf.Atan2(y0, x0);
            }
        }
    }

    private const float PI = 3.14159265f;
    private const float PIBy2 = 1.5707963f;
    // |error| < 0.005
    public static float Atan2(float y, float x)
    {
        if (x == 0.0f)
        {
            if (y > 0.0f) return PIBy2;
            if (y == 0.0f) return 0.0f;
            return -PIBy2;
        }
        float atan;
        float z = y / x;
        if (Mathf.Abs(z) < 1.0f)
        {
            atan = z / (1.0f + 0.28f * z * z);
            if (x < 0.0f)
            {
                if (y < 0.0f) return atan - PI;
                return atan + PI;
            }
        }
        else
        {
            atan = PIBy2 - z / (z * z + 0.28f);
            if (y < 0.0f) return atan - PI;
        }
        return atan;
    }
}
