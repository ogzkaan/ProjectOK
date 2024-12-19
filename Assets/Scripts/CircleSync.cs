using UnityEngine;

public class CircleSync : MonoBehaviour
{
    public static int PosID = Shader.PropertyToID("_Position");
    public static int SizeID = Shader.PropertyToID("_Size");
    public Material wallMaterial;
    public Camera camera;
    public LayerMask layerMask;

    void Update()
    {
        var dir = camera.transform.position - transform.forward;
        var ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, 3000,layerMask))
        {
            wallMaterial.SetFloat(SizeID, 1);
        }
        else
        {
            wallMaterial.SetFloat(SizeID, 0);
        }
        var view = camera.WorldToViewportPoint(transform.position);
        wallMaterial.SetVector(PosID, view);
    }
}
