using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Manager : MonoBehaviour
{
    [Header("Instances Variables")]
    [SerializeField] private GameObject _simpleUnit;
    [SerializeField] private List<AI_Class> _instancesList = new List<AI_Class>();


    private Vector3 movingVector;
    // Start is called before the first frame update
    void Start()
    {
        movingVector = new Vector3(0, 0, 1);

        Vector3 offset = Vector3.zero;
        for(int i = 0; i < 300; i++)
        {
            offset += (Vector3.left * 1.5f);
            _instancesList.Add(Instantiate<GameObject>(_simpleUnit, offset, transform.rotation).GetComponent<AI_Class>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        foreach(AI_Class AI in _instancesList)
        {
            AI.Rigidbody.MovePosition(AI.Transform.position + movingVector);
        }
        */
    }
}