using UnityEngine;
using UnityEngine.UI;

public class MinimapMaterialController : MonoBehaviour
{
    [SerializeField] private FogOfWarController fog;
    [SerializeField] private RawImage minimapImage;

    private Material runtimeMaterial;

    private void Start()
    {
        runtimeMaterial = Instantiate(minimapImage.material);
        minimapImage.material = runtimeMaterial;
        runtimeMaterial.SetTexture("_MainTex", minimapImage.texture);
        runtimeMaterial.SetTexture("_FogTex", fog.FogTexture);
        Debug.Log(runtimeMaterial.GetTexture("_FogTex"));
        Debug.Log(runtimeMaterial.GetTexture("_MainTex"));
    }
}
