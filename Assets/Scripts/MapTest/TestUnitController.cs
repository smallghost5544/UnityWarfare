using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnitController : MonoBehaviour
{
    public GridController gridController;
    public GameObject unitPrefab;
    public int numUnitsPerSpawn;
    public float moveSpeed;

    private List<GameObject> unitsInGame;

    private void Awake()
    {
        unitsInGame = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnUnits();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DestroyUnits();
        }
    }

    private void FixedUpdate()
    {
        if (gridController.curFlowField == null) { return; }
        foreach (GameObject unit in unitsInGame)
        {
            Cell cellBelow = gridController.curFlowField.GetCellFromWorldPos(unit.transform.position);
            Vector3 moveDirection = new Vector3(cellBelow.bestDirection.Vector.x,  cellBelow.bestDirection.Vector.y , 0);
            //Rigidbody unitRB = unit.GetComponent<Rigidbody>();
            Rigidbody2D unitRB = unit.GetComponent<Rigidbody2D>();

            unitRB.velocity = moveDirection * moveSpeed;
        }
    }

    private void SpawnUnits()
    {
        Vector2Int gridSize = gridController.gridSize;
        float nodeRadius = gridController.cellRadius;
        Vector2 maxSpawnPos = new Vector2(gridSize.x * nodeRadius * 2 + nodeRadius, gridSize.y * nodeRadius * 2 + nodeRadius);
        int colMask = LayerMask.GetMask("Impassible", "Units");
        Vector3 newPos;
        for (int i = 0; i < numUnitsPerSpawn; i++)
        {
            GameObject newUnit = Instantiate(unitPrefab);
            newUnit.transform.parent = transform;
            unitsInGame.Add(newUnit);
            do
            {
                newPos = new Vector3(Random.Range(0, maxSpawnPos.x), 0, Random.Range(0, maxSpawnPos.y));
                newUnit.transform.position = newPos;
            }
            while (Physics.OverlapSphere(newPos, 0.25f, colMask).Length > 0);
        }
    }

    private void DestroyUnits()
    {
        foreach (GameObject go in unitsInGame)
        {
            Destroy(go);
        }
        unitsInGame.Clear();
    }
}
