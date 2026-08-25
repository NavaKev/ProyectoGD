using UnityEngine;

public class MainCamara : MonoBehaviour
{
    [SerializeField] private Transform objetivo;
    [SerializeField] private float suavizado = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);

    private Vector3 velocidad = Vector3.zero;

    private void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = objetivo.position + offset;
        // SmoothDamp elimina cualquier tirón entre FixedUpdate y el renderizado
        transform.position = Vector3.SmoothDamp(transform.position, posicionDeseada, ref velocidad, suavizado);
    }
}