using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTowerSpawn : MonoBehaviour
{
    
    [Header("Main Tower level count, bro")]
    [Tooltip("Specify Main Tower level count and it radius")]
    [SerializeField] private float[] _radiuses;
    [Header("How many Defense Towers at each Main Tower levels")]
    [SerializeField] private int _count;
    [Header("Size of Gizmos box")]
    [SerializeField] private Vector3 _boxSize;
    [Header("Angle offset for Gizmos boxes")]
    [SerializeField] private float _angleOffset;

    private float _angle = 0;
    private float _divisionColor = 1;
    private Vector3 _position;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < _radiuses.Length; i++)
        {
            _angle = 0 + (_angleOffset * i);
            float floatI = i;
            _divisionColor = 1 - (floatI / _radiuses.Length);
            Color drawColor = new Color(0.7f * _divisionColor, 0.7f * _divisionColor, 1, 1F);
            Gizmos.color = drawColor;
            for (int j = 0; j < _count; j++)
            {
                var z = Mathf.Cos(_angle * Mathf.Deg2Rad) * _radiuses[i];
                var x = Mathf.Sin(_angle * Mathf.Deg2Rad) * _radiuses[i];

                _angle += 360 / _count;

                _position = new Vector3(transform.position.x + x, transform.position.y + 0.5f, transform.position.z + z);
                Gizmos.DrawCube(_position, _boxSize);
            }
        }
        _divisionColor = 0;
        _angle = 0;
    }
}
