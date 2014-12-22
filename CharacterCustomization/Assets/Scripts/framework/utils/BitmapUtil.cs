using UnityEngine;

public static class BitmapUtil
{
    public static Texture2D draw(GameObject go)
    {
        var tx2d = new Texture2D(10, 10, TextureFormat.RGBA32, true);
        return tx2d;
    }
}