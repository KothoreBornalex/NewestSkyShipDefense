using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
        SetUp_SpawnPoint();
    }


    private void SetUp_SpawnPoint()
    {
        RaycastHit hitInfo = new RaycastHit();

        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
        {
            transform.position = hitInfo.point;
        }
    }

}
