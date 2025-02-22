using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarController : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float motorForce = 1500f;
    [SerializeField] private float brakeForce = 5000f;
    [SerializeField] private float waypointTolerance = 3f;

    [SerializeField] private WheelCollider frontLeftWheel;
    [SerializeField] private WheelCollider frontRightWheel;
    [SerializeField] private WheelCollider rearLeftWheel;
    [SerializeField] private WheelCollider rearRightWheel;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    private Rigidbody rb;
    private float currentSteerAngle;
    private bool isBraking = false;
    private bool isReversing = false;
    private float stuckTimer = 0f;
    private const float stuckThreshold = 3f; // Время ожидания перед включением заднего хода

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        CheckIfStuck();
        Drive();
        HandleSteering();
        UpdateWheels(); // Добавляем обновление колес
    }

    private void Drive()
    {
        if (waypoints.Length == 0) return;

        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance < waypointTolerance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        float motorInput = isReversing ? -1f : 1f; // Если застряли, включаем задний ход

        // Торможение в повороте (если угол поворота больше 20°)
        isBraking = Mathf.Abs(currentSteerAngle) > 20f && !isReversing;

        float brakeInput = isBraking ? brakeForce : 0f;

        frontLeftWheel.motorTorque = motorInput * motorForce;
        frontRightWheel.motorTorque = motorInput * motorForce;

        ApplyBraking(brakeInput);
    }

    private void ApplyBraking(float brakeInput)
    {
        frontRightWheel.brakeTorque = brakeInput;
        frontLeftWheel.brakeTorque = brakeInput;
        rearLeftWheel.brakeTorque = brakeInput;
        rearRightWheel.brakeTorque = brakeInput;
    }

    private void HandleSteering()
    {
        if (waypoints.Length == 0) return;

        Vector3 relativeVector = transform.InverseTransformPoint(waypoints[currentWaypointIndex].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        // Если задний ход, разворачиваем в другую сторону
        if (isReversing)
        {
            newSteer = -newSteer;
        }

        frontLeftWheel.steerAngle = newSteer;
        frontRightWheel.steerAngle = newSteer;

        currentSteerAngle = newSteer;
    }

    private void CheckIfStuck()
    {
        if (rb.velocity.magnitude < 0.5f) // Если машина почти не движется
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > stuckThreshold)
            {
                isReversing = true;
            }
        }
        else
        {
            stuckTimer = 0f;
            isReversing = false;
        }
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheel, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheel, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheel, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheel, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
}


