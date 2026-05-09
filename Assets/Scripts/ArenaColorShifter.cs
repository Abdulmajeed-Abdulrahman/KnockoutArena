using UnityEngine;

public class ArenaColorShifter : MonoBehaviour
{
    private MeshRenderer arenaRenderer;
    private Material arenaMaterial;

    [SerializeField] [Tooltip("List of colors the arena will cycle through for each wave.")]
    private Color[] waveColors;

    [SerializeField] [Tooltip("How fast the color should transition.")]
    private float lerpSpeed = 2f;

    private Color targetColor;

    void Awake()
    {
        // Cache the renderer and create a unique instance of the material
        arenaRenderer = GetComponent<MeshRenderer>();
        arenaMaterial = arenaRenderer.material; 
        
        // Start with the first color in the list
        if (waveColors.Length > 0)
        {
            targetColor = waveColors[0];
            arenaMaterial.SetColor("_BaseColor", targetColor);
        }
    }

    void Update()
    {
        // Smoothly transition the color over time for a polished feel
        Color currentColor = arenaMaterial.GetColor("_BaseColor");
        arenaMaterial.SetColor("_BaseColor", Color.Lerp(currentColor, targetColor, Time.deltaTime * lerpSpeed));
    }

    public void ChangeArenaColor(int waveNumber)
    {
        if (waveColors.Length == 0) return;

        // Use the modulo operator to loop back to the start of the array if waves exceed the list
        int colorIndex = (waveNumber - 1) % waveColors.Length;
        targetColor = waveColors[colorIndex];
    }
}