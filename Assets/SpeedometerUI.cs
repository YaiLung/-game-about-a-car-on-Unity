using UnityEngine;
using UnityEngine.UI;

public class SpeedometerUI : MonoBehaviour
{
    [SerializeField] private Rigidbody carRigidbody;
    [SerializeField] private Text speedText;
    [SerializeField] private RectTransform speedArrow;

    [SerializeField] private float maxSpeed = 200f; // Максимальная скорость машины
    [SerializeField] private float maxRotation = -220f; // Максимальный угол стрелки (на 200 км/ч)
    [SerializeField] private float minRotation = 0f; // Минимальный угол стрелки (на 0 км/ч)

    private void Update()
    {
        float speed = carRigidbody.velocity.magnitude * 3.6f; // Преобразуем м/с в км/ч

        // Обновляем текст
        speedText.text = Mathf.RoundToInt(speed) + " km/h";

        // Вычисляем угол стрелки
        float rotationZ = Mathf.Lerp(minRotation, maxRotation, speed / maxSpeed);
        speedArrow.localRotation = Quaternion.Euler(0, 0, rotationZ);
    }
}

