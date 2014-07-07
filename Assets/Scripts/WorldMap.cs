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

    private bool _isFullScreen;

	void Start () 
	{
		_player = GameObject.FindGameObjectWithTag("Player");
	}

	void Update () 
	{
	    if (InputManager.GetButtonDown(InputMapping.Map))
	    {
	        _isFullScreen = !_isFullScreen;
            GameBrain.CurrentView = _isFullScreen 
                ? GameView.Map 
                : GameView.Internal;
	    }

	    if (_isFullScreen)
	    {
	        camera.rect = new Rect(0, 0, 1, 1);
	        camera.aspect = (float)Screen.width / Screen.height;
	        camera.orthographicSize = 3000;
            camera.transform.position = new Vector3(0, 500, 0);
            
	    }
        else
        {
            camera.pixelRect = new Rect(Screen.width - Padding - MapSize, Screen.height - Padding - MapSize, MapSize, MapSize);
            camera.orthographicSize = 200;
            camera.aspect = 1;

            // track player
            var pos = _player.transform.position;
            pos.y = 500;
            camera.transform.position = pos;
        }
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
