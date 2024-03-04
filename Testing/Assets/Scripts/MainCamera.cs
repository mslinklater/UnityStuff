using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public enum CameraMode {
        Normal,
        DebugFly
    }

    public enum DebugCamMode {
        Fly,
        NoFly
    }

    Vector3 normalPosition = new Vector3();
    Quaternion normalRotation = new Quaternion();

    Vector3 debugFlyPosition = new Vector3();
    Quaternion debugFlyRotation = new Quaternion();

    public CameraMode mode = CameraMode.Normal;
    public DebugCamMode debugCamMode = DebugCamMode.NoFly;

    private float forwardInput;
    private float strafeInput;
//    private float yawInput;
//    private float pitchInput;
//    private float pitch = 0.0f;
//    private float yaw = 0.0f;
    private float pitchDiff = 0.0f;
    private float yawDiff = 0.0f;

    private Vector3 lastMousePosition;
    private Vector3 buttonDownPos;
    private Vector3 flyBaseOrient;

    void Start()
    {
        normalPosition = transform.position;
        normalRotation = transform.rotation;
        debugFlyPosition = transform.position;
        debugFlyRotation = transform.rotation;

        lastMousePosition = Input.mousePosition;
        flyBaseOrient = new Vector3(0.0f, 0.0f, 0.0f);
    }

    void ProcessInput()
    {
        forwardInput = Input.GetAxis("Vertical");
        strafeInput = Input.GetAxis("Horizontal");
    }

    private void UpdateNormal()
    {
        // first person on the player capsule
    }

    private void UpdateDebugFly()
    {
        float dt = Time.deltaTime;

        ProcessInput();

        if(Input.GetMouseButtonDown(1))
        {
            buttonDownPos = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(1))
        {
            flyBaseOrient += new Vector3(pitchDiff, yawDiff, 0.0f);
        }

        Vector3 pos = debugFlyPosition;
        pos += transform.forward * forwardInput * dt * 5.0f;
        pos += transform.right * strafeInput * dt * 5.0f;
        debugFlyPosition = pos;

        if(Input.GetMouseButton(1)) // right button held down
        {
            pitchDiff = (buttonDownPos.y - Input.mousePosition.y) * 0.1f;
            yawDiff = (Input.mousePosition.x - buttonDownPos.x) * 0.1f;

            Quaternion quat = Quaternion.Euler(flyBaseOrient.x + pitchDiff, flyBaseOrient.y + yawDiff, 0.0f);

            debugFlyRotation = quat;
        }
        else
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.Space))
        {
            if( mode == CameraMode.Normal)
            {
                mode = CameraMode.DebugFly;

            }
            else
            {
                mode = CameraMode.Normal;
            }
        }

        switch(mode)
        {
            case CameraMode.Normal:
                UpdateNormal();
                transform.position = normalPosition;
                transform.rotation = normalRotation;
                break;
            case CameraMode.DebugFly:
                UpdateDebugFly();
                transform.position = debugFlyPosition;
                transform.rotation = debugFlyRotation;
                break;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Camera:" + mode.ToString());
    }

    void OnDrawGizmos()
    {
        if(mode == CameraMode.DebugFly)
        {
            Gizmos.DrawWireSphere(normalPosition, 0.5f);
        }
    }
}
