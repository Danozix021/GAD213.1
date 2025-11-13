using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    float originalTimeScale;
    float originalFixedDelta;

    private void Awake() // Using Singleton design pattern to ensure that there is only one instance of the GameManager throughout the game.
    {
        instance = this;
        originalTimeScale = 1f;
        originalFixedDelta = Time.fixedDeltaTime;

    }

    public void SlowTime(float targetScale, float durationSeconds)
    {
        Debug.Log("Time Slower picked up");
        Time.timeScale = targetScale;
        Time.fixedDeltaTime = originalFixedDelta * targetScale;

        StartCoroutine(SlowMotionRoutine(durationSeconds)); // Using Coroutine to not allow the duration of the slow motion to get affected by it self
    }

    IEnumerator SlowMotionRoutine(float durationSeconds)
    {
        yield return new WaitForSeconds(durationSeconds);

        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDelta;
    }
}
