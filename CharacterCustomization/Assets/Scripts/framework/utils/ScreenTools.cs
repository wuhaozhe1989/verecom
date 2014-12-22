using UnityEngine;

public class ScreenTools : MonoBehaviour
{
    private static int _baseWidth = 1024;
    private static int _baseHeight = 768;
    //设置基准宽度
    public static void setBaseWidth(int width)
    {
        _baseWidth = width;
    }

    //设置基准高度
    public static void seBaseHeight(int height)
    {
        _baseHeight = height;
    }

    //获取相对于原有基准设置的宽或y
    public static float getLocalX(float x)
    {
        return x/_baseWidth*Screen.width;
    }

    //获取相对于基准的高或者y
    public static float getLocalY(float y)
    {
        return y/_baseHeight*Screen.height;
    }
}