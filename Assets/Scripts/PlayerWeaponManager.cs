using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerWeaponManager : MonoBehaviour
{
    private MissileLauncher _missileLauncher;
    private BombLauncher _bombLauncher;

    List<IPlayerWeapon> _weapons = new List<IPlayerWeapon>();
    int _selectedWeaponIndex;

    // Use this for initialization
    void Start()
    {
        _missileLauncher = GetComponent<MissileLauncher>();
        _bombLauncher = GetComponent<BombLauncher>();

        _weapons.Add(_missileLauncher);
        _weapons.Add(_bombLauncher);
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetButtonDown(InputMapping.ChangeWeapon))
        {
            _selectedWeaponIndex = (_selectedWeaponIndex + 1) % _weapons.Count;
        }

        if (InputManager.GetButtonDown(InputMapping.FireSecondary))
        {
            _weapons[_selectedWeaponIndex].Fire();
        }
    }

    void OnGUI()
    {
        if (!GameBrain.HideHud)
        {
            _weapons[_selectedWeaponIndex].DrawHud();
        }
    }
}

public interface IPlayerWeapon
{
    void DrawHud();
    void Fire();
}