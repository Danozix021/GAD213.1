using System.Collections.Generic;
using UnityEngine;

public class DoubleJump : MonoBehaviour, ICollectible
{
    public void OnCollect(Collector collector)
    {
        Debug.Log("Double jump picked up.");
        var movement = collector.GetComponent<PlayerMovementController>();
        movement.DoubleJump();
        Destroy(gameObject);
    }

}

