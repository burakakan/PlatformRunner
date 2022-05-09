using UnityEngine;

public class Painter : MonoBehaviour
{
    [SerializeField]
    private GameObject paintPuff;
    [SerializeField]
    private RenderTexture wallRenderTexture;
    [SerializeField]
    private Material canvasMaterial;
    [SerializeField]
    private Camera mainCamera, canvasCamera;
    [SerializeField]
    private float puffSize = 1f;
    [SerializeField]
    private Color puffColor = Color.red;
    [SerializeField]
    private Texture whiteTexture;

    private RaycastHit hit;
    private Ray sprayRay;
    private float halfSize;
    private Texture2D canvasTexture;

    private void Awake()
    {
        PlayerInput.SwerveFree += Paint;
        halfSize = canvasCamera.orthographicSize;
    }
    private void Start()
    {
        //create the editable canvas texture and assign it to the canvas material
        canvasTexture = new Texture2D(wallRenderTexture.width, wallRenderTexture.height, TextureFormat.RGB24, false, true);
        Graphics.CopyTexture(whiteTexture, 0, 0, 0, 0, whiteTexture.width, whiteTexture.height, canvasTexture, 0, 0, 0, 0);
        canvasMaterial.mainTexture = canvasTexture;

        paintPuff.GetComponent<SpriteRenderer>().color = puffColor;
        paintPuff.transform.localScale = puffSize * Vector3.one;
    }
    void Paint(Vector2 touchPosition)
    {
        //cast a ray from touch to the game world
        sprayRay = mainCamera.ScreenPointToRay(touchPosition);
        //if the ray doesn't hit anything or the collider it hits doesn't have a mesh, don't do anything
        if (!Physics.Raycast(sprayRay, out hit, 100)/* || (hit.collider as MeshCollider).sharedMesh == null*/)
            return;
        //Debug.Log("mc: " + hit.collider == null);
        //move the sprite to the uv coordinates that correspond to the touched point, subtract the halfsize of the orthographic camera to correct for the difference between texture and world coordinates
        paintPuff.transform.localPosition = new Vector3(hit.textureCoord.x - halfSize, hit.textureCoord.y - halfSize);
        //permanently apply the sprite to the canvas
        Burn();
    }
    private void Burn()
    {
        RenderTexture.active = wallRenderTexture;
        canvasTexture.ReadPixels(new Rect(0, 0, wallRenderTexture.width, wallRenderTexture.height), 0, 0);
        canvasTexture.Apply();
        RenderTexture.active = null;
    }
    private void OnApplicationQuit()
    {
        canvasMaterial.mainTexture = whiteTexture;
    }

}
