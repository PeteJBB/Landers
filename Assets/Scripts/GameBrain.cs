using UnityEngine;
using System.Collections;
using System.Linq;

public class GameBrain : MonoBehaviour
{
    public static GameView CurrentView = GameView.Internal;
    public static bool HideHud = false;

    public GameObject ScoutPrefab;
    public GameObject AttackPlanePrefab;
    public GameObject DropShipPrefab;

    public int CurrentWave = 0;
    public int CurrentEnemies = 0;

    private float _waveCompleteTime;
    private bool _isWaveComplete = true;

    private const float _timeBetweenWaves = 10;

    private const float LanderSpawnDelay = 20;
    
    private int _spawnCount = 0;
    private float _lastSpawn;

    void Start()
    {
        _waveCompleteTime = Time.fixedTime - 5;
        _lastSpawn = Time.fixedTime;
    }


    void Update()
    {
        if (Time.fixedTime - _lastSpawn > LanderSpawnDelay)
        {
            //GetComponent<LanderFactory>().SpawnLander();
            _lastSpawn = Time.fixedTime;
        }

        CurrentEnemies = FindObjectsOfType<TeamMember>().Where(x => x.Team == 2).Count();

        if (_isWaveComplete)
        {
            if (Time.fixedTime - _waveCompleteTime > _timeBetweenWaves)
            {
                // next wave
                CurrentWave++;
                _isWaveComplete = false;

                // spawn ships
                Instantiate(ScoutPrefab, new Vector3(1201, 180, -1499), Quaternion.Euler(0, 317, 0));
                Instantiate(ScoutPrefab, new Vector3(1220, 180, -1505), Quaternion.Euler(0, 317, 0));
                Instantiate(AttackPlanePrefab, new Vector3(1205, 180, -1519), Quaternion.Euler(0, 317, 0));
                Instantiate(AttackPlanePrefab, new Vector3(1207, 180, -1538), Quaternion.Euler(0, 317, 0));

                // spawn dropship
                var sites = FindObjectsOfType<LandingSite>().Where(x => !x.IsEngaged).ToArray();
                if (sites.Length > 0)
                {
                    var site = sites[Random.Range(0, sites.Length)];
                    var pos = site.transform.position + (site.ApproachVector.normalized * 2000);
                    pos.y = 1000;

                    var dropship = (GameObject)Instantiate(DropShipPrefab, pos, Quaternion.identity);
                    dropship.transform.position = pos;
                    dropship.GetComponent<Dropship>().LandingSite = site;
                    site.IsEngaged = true;
                }
            }
        }
        else if (CurrentEnemies == 0)
        {
            _isWaveComplete = true;
            _waveCompleteTime = Time.fixedTime;
        }
    }

    private void OnGUI()
    {
        if (_isWaveComplete)
        {
            var message = CurrentWave == 0
                ? "Get ready for first wave"
                : "You got 'em! Prepare for Wave " + (CurrentWave + 1) + "!";

            var rext = Utility.GetCenteredRectangle(new Vector2(Screen.width / 2f, Screen.height / 3f), 320, 64);
            GUI.TextArea(rext, message, GuiStyles.BigMessageGuiStyle);
        }
    }

}

public enum GameView
{
    Internal,
    External,
    Map,
    Menu
}