using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxControl : MonoBehaviour
{
    public float exitTime;
    // Start is called before the first frame update
    void Start()
    {
        exitTime = 0.5f;

    }
    void Update()
    {
        exitTime -= Time.deltaTime;
        if (exitTime <= 0)
            Destroy(gameObject);
    }
}
