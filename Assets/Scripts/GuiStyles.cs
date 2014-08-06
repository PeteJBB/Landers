using UnityEngine;
using System.Collections;

public static class GuiStyles
{
    public static GUIStyle BasicGuiStyle = new GUIStyle()
    {
        clipping = TextClipping.Overflow,
        wordWrap = false,
        normal = new GUIStyleState()
        {
            textColor = Color.white
        }
    };

    public static GUIStyle BasicGuiStyleCentered = new GUIStyle()
    {
        clipping = TextClipping.Overflow,
        wordWrap = false,
        normal = new GUIStyleState()
        {
            textColor = Color.white
        },
        alignment = TextAnchor.MiddleCenter
    };

    public static GUIStyle BigMessageGuiStyle = new GUIStyle()
    {
        clipping = TextClipping.Overflow,
        wordWrap = false,
        normal = new GUIStyleState()
        {
            textColor = Color.white,
            //background = Utility.CreatePlainColorTexture(Color.black)
        },
        alignment = TextAnchor.MiddleCenter,
        fontSize = 30
    };

    public static GUIStyle Hud_Speed = new GUIStyle()
    {
        clipping = TextClipping.Clip,
        wordWrap = false,
        normal = new GUIStyleState()
        {
            textColor = Color.white
        },
        padding = new RectOffset(6, 6, 6, 6),
        alignment = TextAnchor.MiddleRight,
        fontSize = 16,
        font = (Font)Resources.Load("Fonts/CONSOLA")
    };

    public static GUIStyle Hud_Label = new GUIStyle()
    {
        clipping = TextClipping.Overflow,
        wordWrap = false,
        normal = new GUIStyleState()
        {
            textColor = Color.white
        },
        alignment = TextAnchor.MiddleLeft,
        fontSize = 14,
        fontStyle = FontStyle.Bold,
        font = (Font)Resources.Load("Fonts/CONSOLA")
    };

    public static GUIStyle Hud_Label_Right = new GUIStyle()
    {
        clipping = TextClipping.Overflow,
        wordWrap = false,
        normal = new GUIStyleState()
        {
            textColor = Color.white
        },
        alignment = TextAnchor.MiddleRight,
        fontSize = 14,
        fontStyle = FontStyle.Bold,
        font = (Font)Resources.Load("Fonts/CONSOLA")
    };

    public static GUIStyle Hud_Label_Small = new GUIStyle()
    {
        clipping = TextClipping.Overflow,
        wordWrap = false,
        normal = new GUIStyleState()
        {
            textColor = Color.white
        },
        alignment = TextAnchor.MiddleLeft,
        fontSize = 10,
        fontStyle = FontStyle.Bold,
        font = (Font)Resources.Load("Fonts/CONSOLA")
    };
}
