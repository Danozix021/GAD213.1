using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float followSpeed = 1f;
    public bool usePlayerSpeed = true;

    [Header("Constraints")]
    public bool followHorizontally = true;
    public bool followVertically = false;
    public float fixedYPosition = 0f;

    [Header("Optional Settings")]
    public Vector2 offset = Vector2.zero;
    public float smoothTime = 0.1f;
    public bool useSmoothDamping = false;

    private Vector3 velocity = Vector3.zero;
    private PlayerMovementController playerMovement;
    private float lastTargetX;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        if (target != null)
        {
            playerMovement = target.GetComponent<PlayerMovementController>();
        }

        if (target != null)
        {
            lastTargetX = target.position.x;
            fixedYPosition = transform.position.y;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = CalculatePosition();

        if (useSmoothDamping)
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        }
        else
        {
            float speed = GetFollowSpeed();
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, speed * Time.deltaTime);
        }
    }

    private Vector3 CalculatePosition()
    {
        Vector3 targetPosition = target.position;
        Vector3 currentPosition = transform.position;

        float desiredX = followHorizontally ? targetPosition.x + offset.x : currentPosition.x;
        float desiredY = followVertically ? targetPosition.y + offset.y : fixedYPosition;
        float desiredZ = currentPosition.z;

        return new Vector3(desiredX, desiredY, desiredZ);
    }

    private float GetFollowSpeed()
    {
        if (usePlayerSpeed && playerMovement != null)
        {
            float playerVelocityX = Mathf.Abs(target.position.x - lastTargetX) / Time.deltaTime;
            lastTargetX = target.position.x;
            return Mathf.Max(playerVelocityX, playerMovement.maxSpeed);
        }

        return followSpeed;
    }
}
