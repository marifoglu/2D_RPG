using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{

    private Camera mainCamera;
    private float lastCameraPositionX;  
    private float cameraHalfWidth;

    [SerializeField] private ParallaxLayer[] backgroundLayers;

    private void Awake()
    {
        mainCamera = Camera.main;
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        InitializeLayers();
    }

    private void FixedUpdate()
    {
        float currentCameraPositionx = mainCamera.transform.position.x;
        float distanceToMove = currentCameraPositionx - lastCameraPositionX; 
        lastCameraPositionX = currentCameraPositionx;
    
        float cameraLeftEdge = currentCameraPositionx - cameraHalfWidth;
        float cameraRightEdge = currentCameraPositionx + cameraHalfWidth;
        

        foreach (ParallaxLayer layer in backgroundLayers)
        {
            layer.Move(distanceToMove);
            layer.LoopBackground(cameraLeftEdge, cameraRightEdge);
        }
    }

    private void InitializeLayers()
    {
        foreach (ParallaxLayer layer in backgroundLayers)
        {
            layer.calculateImageWidth();
        }
    }



}
