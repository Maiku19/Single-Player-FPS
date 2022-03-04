using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameScreen : MonoBehaviour
{
    [SerializeField] GameObject victory;
    [SerializeField] GameObject defeat;

    public void OnTeam1Victory()
    {
        victory.SetActive(true);
    }
    
    public void OnTeam2Victory()
    {
        defeat.SetActive(true);
    }
}
