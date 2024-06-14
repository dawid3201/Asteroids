using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    [Range(-1f, 1f)]
    
    private float offset;
    public float scrollSpeed = 0.5f;
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        offset += (Time.deltaTime * scrollSpeed) / 10f;
        mat.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}