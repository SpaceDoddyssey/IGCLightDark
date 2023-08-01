using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    public float scrollSpeed = 0.1f;
    private Material material;
    private GameState stateObject;

    private void Start()
    {
        // Get the material component from the Renderer
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;
        stateObject = GameObject.Find("Game World Manager").GetComponent<GameState>();
    }

    private void Update()
    {
        // Calculate the new texture offset
        float offset = 0f;
        if (stateObject.polarity == 0) return;
        offset = Time.time * scrollSpeed * 0.4f * stateObject.GetGradientSpeed() * Mathf.Sign(stateObject.polarity);

        Vector2 offsetVec = new Vector2(0f, offset);

        // Assign the offset to the material
        material.SetTextureOffset("_MainTex", offsetVec);
    }
}