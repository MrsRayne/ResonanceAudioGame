using System.Linq;
using UnityEngine;

public class ListenerReverbManipulator : MonoBehaviour
{
    [Range(0,360)]
    public int raycasts = 12;
    public LayerMask raycastMask = 0;
    public float reflectionsLevelVariance = 2000f;
    public float reflectionsDelayVariance = 0.18f;
    public float reverbLevelVariance = -800f;
    public Vector2 manipulationRange = new Vector2(0.5f, 9f);

    AudioReverbZone audioReverbZone;
    float[] raycastLengths;
    float average;
    float minimum;
    float originalReflectionsLevel;
    float originalReflectionsDelay;
    float originalReverbLevel;

    void Start()
    {
        raycastLengths = new float[raycasts];
        audioReverbZone = FindObjectOfType<AudioReverbZone>();
        originalReflectionsLevel = audioReverbZone.reflections;
        originalReflectionsDelay = audioReverbZone.reflectionsDelay;
        originalReverbLevel = audioReverbZone.reverb;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < raycasts; i++)
        {
            Vector3 direction = Quaternion.Euler(0, (360f / (float)raycasts) * i, 0) * Vector3.forward;
            Physics.Raycast(transform.position, direction, out RaycastHit hit, 100f, raycastMask);
            raycastLengths[i] = hit.collider ? Vector3.Distance(hit.point, transform.position) : 0;
        }
        average = raycastLengths.Average();
        minimum = raycastLengths.Min();
        var minimumCapped = Mathf.Clamp(minimum, manipulationRange.x, manipulationRange.y);
        var minimumNormalized = (minimumCapped - manipulationRange.x) / manipulationRange.y;
        var averageCapped = Mathf.Clamp(average, manipulationRange.x, manipulationRange.y);
        var averageNormalized = (averageCapped - manipulationRange.x) / manipulationRange.y;
        audioReverbZone.reflections = Mathf.CeilToInt(originalReflectionsLevel + ((1 - minimumNormalized) * reflectionsLevelVariance));
        audioReverbZone.reflectionsDelay = originalReflectionsDelay + ((1 - minimumNormalized) * reflectionsDelayVariance);
        audioReverbZone.reverb = Mathf.CeilToInt(originalReverbLevel + ((1 - minimumNormalized) * reverbLevelVariance));
    }
}
