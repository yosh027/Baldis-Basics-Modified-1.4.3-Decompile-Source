using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private void Start()
    {
        offset = transform.position - player.transform.position;
        playerController = player.GetComponent<CharacterController>();
        cam = GetComponent<Camera>();
        if (cam != null) cam.fieldOfView = normalFOV;
    }

    private void Update()
    {
        if (ps.jumpRope)
        {
            velocity -= gravity * Time.deltaTime;
            jumpHeight += velocity * Time.deltaTime;
            if (jumpHeight <= 0f)
            {
                jumpHeight = 0f;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    velocity = initVelocity;
                }
            }
            jumpHeightV3 = new Vector3(0f, jumpHeight, 0f);
        }
        else if (Input.GetButton("Look Behind"))
        {
            lookBehind = 180;
        }
        else
        {
            lookBehind = 0;
        }

        bool moving = IsPlayerMoving();
        bool running = moving && Input.GetKey(KeyCode.LeftShift);

        if (moving)
        {
            float speedMultiplier = running ? runMultiplier : 1f;
            bobTimer += Time.deltaTime * bobFrequency * speedMultiplier;

            // Reset idle when moving
            idlePosTimer = 0f;
            idleCurrentRot = 0f;
            idleRotIncreasing = true;
        }
        else
        {
            idlePosTimer += Time.deltaTime * idlePosFrequency;

            // Linear idle X rotation
            if (idleRotIncreasing)
            {
                idleCurrentRot += idleRotSpeed * Time.deltaTime;
                if (idleCurrentRot >= 2f)
                {
                    idleCurrentRot = 2f;
                    idleRotIncreasing = false;
                }
            }
            else
            {
                idleCurrentRot -= idleRotSpeed * Time.deltaTime;
                if (idleCurrentRot <= 0f)
                {
                    idleCurrentRot = 0f;
                    idleRotIncreasing = true;
                }
            }

            // Reset bob when idle
            bobTimer = 0f;
        }

        // FOV transition (uses run multiplier for FOV too)
        if (cam != null)
        {
            float targetFOV = normalFOV * (running ? runFOVMultiplier : 1f);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
        }
    }

    private void LateUpdate()
    {
        Vector3 targetPos = player.transform.position + offset;
        Quaternion targetRot = player.transform.rotation * Quaternion.Euler(0f, lookBehind, 0f);

        if (!ps.gameOver && !ps.jumpRope)
        {
            float yOffset = 0f;
            float xIdleRotation = 0f;

            bool moving = IsPlayerMoving();
            bool running = moving && Input.GetKey(KeyCode.LeftShift);
            float speedMultiplier = running ? runMultiplier : 1f;

            if (moving)
            {
                // Bobbing (affected by run multiplier)
                yOffset += Mathf.Sin(bobTimer) * bobAmplitude * speedMultiplier;
            }
            else
            {
                // Idle breathing
                yOffset += Mathf.Sin(idlePosTimer) * idlePosAmplitude;

                // Idle X rotation
                xIdleRotation = idleCurrentRot;
            }

            targetPos.y += yOffset;
            Quaternion idleRotOffset = Quaternion.Euler(xIdleRotation, 0f, 0f);

            transform.position = targetPos;
            transform.rotation = targetRot * idleRotOffset;
        }
        else if (ps.gameOver)
        {
            transform.position = baldi.transform.position + baldi.transform.forward * 2f + new Vector3(0f, 5f, 0f);
            transform.LookAt(new Vector3(baldi.position.x, baldi.position.y + 5f, baldi.position.z));
        }
        else if (ps.jumpRope)
        {
            transform.position = player.transform.position + offset + jumpHeightV3;
            transform.rotation = player.transform.rotation;
        }
    }

    private bool IsPlayerMoving()
    {
        if (playerController != null)
        {
            return playerController.velocity.magnitude > 0.1f;
        }
        else
        {
            return Input.GetAxis("Strafe") != 0f || Input.GetAxis("Forward") != 0f;
        }
    }

	public GameObject player;
    public PlayerScript ps;
    public Transform baldi;
    public float initVelocity;
    public float velocity;
    public float gravity;
    private int lookBehind;
    public Vector3 offset;
    public float jumpHeight;
    public Vector3 jumpHeightV3;
    private CharacterController playerController;
    public float bobFrequency = 6f;
    public float bobAmplitude = 0.05f;
    public float runMultiplier = 1.5f;
    private float bobTimer;
    public float idlePosFrequency = 1f;
    public float idlePosAmplitude = 0.02f;
    private float idlePosTimer;
    public float idleRotSpeed = 1f;
    private float idleCurrentRot;
    private bool idleRotIncreasing = true;
    private Camera cam;
    public float normalFOV = 60f;
    public float runFOVMultiplier = 1.1f;
    public float fovTransitionSpeed = 5f;

}