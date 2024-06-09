using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform TeamOneRespawnPoint;
    public Transform TeamTwoRespawnPoint;
    public GameObject TeamOneUnit;
    public GameObject TeamTwoUnit;
    public TextMeshProUGUI onStageCountText;

    int enemyCount = 0;
    void Update()
    {
        onStageCountText.text = "Units on stage: " + enemyCount;
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
            enemyCount += 1;
        }
        if (Input.GetKey(KeyCode.N))
        {
            Instantiate(TeamTwoUnit, TeamTwoRespawnPoint);
            enemyCount += 1;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SetGameSpeed(-1.0f);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SetGameSpeed(1.0f);
        }
    }
    void SetGameSpeed(float newSpeed)
    {
        Time.timeScale += newSpeed;
    }
}
