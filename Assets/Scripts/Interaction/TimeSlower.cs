using UnityEngine;

public class TimeSlower : MonoBehaviour, ICollectible
{
    public void OnCollect(Collector collector)
    {
        GameManager.instance.SlowTime(0.4f, 1.5f);
        Destroy(gameObject);
    }
}
