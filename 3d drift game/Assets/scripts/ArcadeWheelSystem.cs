using UnityEngine;
using static CarController;

[System.Serializable]
public class DriftWheel
{
    public Transform wheelTransform; // The visual wheel
    public bool isSteerable;         // Front wheels
    public bool isRearWheel;         // Rear wheels
    public TrailRenderer driftTrail; // Optional trail for drifting
}

public class ArcadeWheelSystem : MonoBehaviour
{
    public DriftWheel[] wheels;
    public Rigidbody carRb;

    [Header("Steering")]
    public float maxSteerAngle = 30f;
    public float steerSpeed = 5f;

    [Header("Wheel Rotation")]
    public float wheelRadius = 0.35f; // in meters

    [Header("Drift Trails")]
    public float driftSpeedThreshold = 5f; // min sideways speed to show trails

    private float horizontalInput;
    private float verticalInput;

    void Update()
    {
        GetInputs();
        RotateWheels();
        HandleDriftTrails();
    }

    void FixedUpdate()
    {
        ApplySteering();
    }

    void GetInputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    void ApplySteering()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.isSteerable)
            {
                float targetAngle = horizontalInput * maxSteerAngle;
                Vector3 localEuler = wheel.wheelTransform.localEulerAngles;
                localEuler.y = Mathf.LerpAngle(localEuler.y, targetAngle, Time.fixedDeltaTime * steerSpeed);
                wheel.wheelTransform.localEulerAngles = new Vector3(localEuler.x, localEuler.y, localEuler.z);
            }
        }
    }

    void RotateWheels()
    {
        float distanceTravelled = carRb.linearVelocity.magnitude * Time.deltaTime;
        float rotationAngle = distanceTravelled / (2 * Mathf.PI * wheelRadius) * 360f;

        foreach (var wheel in wheels)
        {
            wheel.wheelTransform.Rotate(rotationAngle, 0, 0, Space.Self);
        }
    }

    void HandleDriftTrails()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(carRb.linearVelocity);
        float sidewaysSpeed = Mathf.Abs(localVelocity.x);

        foreach (var wheel in wheels)
        {
            if (wheel.driftTrail)
            {
                if (wheel.isRearWheel && sidewaysSpeed > driftSpeedThreshold)
                    wheel.driftTrail.emitting = true;
                else
                    wheel.driftTrail.emitting = false;
            }
        }
    }
}