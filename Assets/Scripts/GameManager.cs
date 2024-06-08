using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform TeamOneRespawnPoint;
    public Transform TeamTwoRespawnPoint;
    public GameObject TeamOneUnit;
    public GameObject TeamTwoUnit;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Instantiate(TeamOneUnit , TeamOneRespawnPoint);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Instantiate(TeamTwoUnit, TeamTwoRespawnPoint);
        }
        if (Input.GetKey(KeyCode.B))
        {
            Instantiate(TeamOneUnit, TeamOneRespawnPoint);
        }
        if (Input.GetKey(KeyCode.N))
        {
            Instantiate(TeamTwoUnit, TeamTwoRespawnPoint);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SetGameSpeed(1.0f);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SetGameSpeed(2.0f);
        }
    }
    void SetGameSpeed(float newSpeed)
    {
        Time.timeScale = newSpeed;
    }
}
