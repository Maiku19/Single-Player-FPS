using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    static AmmoCounter instance;

    int ammo = 0;
    int maxAmmo = 0;

    private void Awake()
    {
        instance = this;
    }

    public static void SetAmmo(int ammount)
    {
        instance.ammo = ammount;
        UpdateCounter();
    }

    public static void SetMaxAmmo(int ammount)
    {
        instance.maxAmmo = ammount;
        UpdateCounter();
    }

    static void UpdateCounter()
    {
        instance.GetComponent<TextMeshProUGUI>().text = instance.ammo + " / " + instance.maxAmmo;
    }
}
