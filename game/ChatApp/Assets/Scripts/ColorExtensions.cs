using UnityEngine;

public static class ColorExtensions
{
    public static Color ToUColor(this System.Drawing.Color color) =>
        new() {a = Norm(color.A), r = Norm(color.R), g = Norm(color.G), b = Norm(color.B)};

    public static System.Drawing.Color ToColor(this Color color) =>
        System.Drawing.Color.FromArgb(ToHex(color.a), ToHex(color.r), ToHex(color.g), ToHex(color.b));

    static float Norm(int value) =>
        value / 255f;

    static int ToHex(float value) =>
        (int) (value * 255);
}