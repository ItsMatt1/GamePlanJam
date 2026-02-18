using UnityEngine;

/// <summary>
/// Attach to a Quad with a repeating-texture material.
/// Keeps the background centered on the camera and tiles the texture via UV offset,
/// so it looks infinite no matter how far the player moves.
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class TiledBackground : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("How many times the 16x16 tile repeats across the visible area.")]
    public float tilesPerUnit = 2f;

    [Tooltip("Extra padding multiplier so the quad is always larger than the camera view.")]
    public float sizeMultiplier = 3f;

    private Material mat;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        mat = GetComponent<MeshRenderer>().material;

        FitToCamera();
    }

    void LateUpdate()
    {
        // Keep centered on camera
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);

        // Scroll UVs based on camera position so the texture tiles seamlessly
        float offsetX = cam.transform.position.x * tilesPerUnit;
        float offsetY = cam.transform.position.y * tilesPerUnit;
        mat.mainTextureOffset = new Vector2(offsetX, offsetY);
    }

    void FitToCamera()
    {
        float height = cam.orthographicSize * 2f * sizeMultiplier;
        float width = height * cam.aspect;

        transform.localScale = new Vector3(width, height, 1f);

        // Set tiling based on world size
        float tilingX = width * tilesPerUnit;
        float tilingY = height * tilesPerUnit;
        mat.mainTextureScale = new Vector2(tilingX, tilingY);
    }
}
