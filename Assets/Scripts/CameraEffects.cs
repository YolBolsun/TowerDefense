using System.Collections;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    private Vector3 initialCameraPosition;
    private float currentMagnitude;

    [SerializeField] private float cameraShakeReductionRate;


    public void CameraShake(float magnitude)
    {
        //advice: for every 1 damage taken, the magnitude should probably be 0.1f or 0.05f
        StartCoroutine(ShakeRoutine(magnitude));
    }

    private IEnumerator ShakeRoutine(float magnitude)
    {

        currentMagnitude += magnitude;
        while (currentMagnitude > 0)
        {
            transform.position = initialCameraPosition + Random.insideUnitSphere * currentMagnitude;
            currentMagnitude -= cameraShakeReductionRate;
            yield return null;
        }
    }

    void Awake()
    {
        initialCameraPosition = transform.position;
        currentMagnitude = 0;
    }
}
