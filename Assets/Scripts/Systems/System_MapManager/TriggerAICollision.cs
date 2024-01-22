using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAICollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<AI_Class>(out AI_Class ai))
        {
            ai.CapsuleCollider.enabled = true;
            ai.NavMeshAgent.radius = ai.AgentRadius;
        }
    }
}
