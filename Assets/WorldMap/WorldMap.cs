using UnityEngine;
using System.Collections;

public class WorldMap : MonoBehaviour 
{
	const float MapSize = 192;
	const float Padding = 10;

	public Texture PositionIndicatior;
    public Texture LanderIcon;
    public Texture PyramidIcon;

	GameObject _player;

	void Start () 
	{
		_player = GameObject.FindGameObjectWithTag("Player");
		camera.aspect = 1;
	}

	void Update () 
	{
        //var ratio = Screen.width / Screen.height;
        //var height = MapSize / Screen.height;
        //var width = height / ratio;
        //var px = 1f - (Padding / Screen.width);
        //var py = 1f - (Padding / Screen.height);
        //camera.rect = new Rect(px - width, py - height, width, height);

        camera.pixelRect = new Rect(Screen.width - Padding - MapSize, Screen.height - Padding - MapSize, MapSize, MapSize);
	}

	void OnGUI()
	{
        DrawPyramids(); 
        DrawPlayerPosition();
	    DrawLanders();
	    
	}

    private void DrawPlayerPosition()
    {
        var pos = camera.WorldToScreenPoint(_player.transform.position);
		pos.y = Screen.height - pos.y;
		var rect = Utility.GetCenteredRectangle(pos, 32, 32);
		Utility.DrawRotatedGuiTexture(rect, _player.transform.eulerAngles.y, PositionIndicatior);
    }

    private void DrawLanders()
    {
        foreach (var g in GameObject.FindGameObjectsWithTag("Lander"))
        {
            var pos = camera.WorldToScreenPoint(g.transform.position);
            pos.y = Screen.height - pos.y;
            var rect = Utility.GetCenteredRectangle(pos, 16, 16);
            GUI.DrawTexture(rect, LanderIcon);
        }
    }

    private void DrawPyramids()
    {
        foreach (var g in GameObject.FindGameObjectsWithTag("Pyramid"))
        {
            var pos = camera.WorldToScreenPoint(g.transform.position);
            pos.y = Screen.height - pos.y;
            var rect = Utility.GetCenteredRectangle(pos, 16, 16);
            GUI.DrawTexture(rect, PyramidIcon);
        }
    }
}
