
using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public interface ISpecialty
{
    bool IsPassive { get; }
    float specializeTime { get; }
    void SetResource( );
    IEnumerator DoSpecialize(Vector3 startPosition , GameObject gameObject, UnitView unitView, UnitStats unitStats);
    void ExecuteAction(Action action);
}
