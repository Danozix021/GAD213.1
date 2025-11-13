using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var collectible = collision.GetComponent<ICollectible>();
        if (collectible != null)
        {
            collectible.OnCollect(this);
        }
    }
}
