using UnityEngine;

public class FovChanger : MonoBehaviour
{
    // The Camera component of the GameObject
    private Camera camera;

    // The normal and target FOV values
    public float normalFov = 60f;
    public float shiftedFov = 90f;

    // The speed at which to change the FOV
    public float fovSmoothSpeed = 5f;

    // Current target FOV
    private float targetFov;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Camera component attached to the GameObject
        camera = GetComponent<Camera>();

        // Set initial FOV to normal FOV
        targetFov = normalFov;
        camera.fieldOfView = targetFov;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the Shift key is held down
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            targetFov = shiftedFov;
        }
        else
        {
            targetFov = normalFov;
        }

        // Smoothly change the FOV towards the target FOV
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFov, fovSmoothSpeed * Time.deltaTime);
    }
}