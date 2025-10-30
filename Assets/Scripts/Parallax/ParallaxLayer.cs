using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]

public class ParallaxLayer
{
    [SerializeField] private Transform background;
    [SerializeField] private float parallaxMultiplier;
    [SerializeField] private float imageWidthOffset = 10;

    private float imageFullWidth;
    private float imageHalfWidth;
    
    public void calculateImageWidth()
    {
        imageFullWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
        imageHalfWidth = imageFullWidth / 2;
    }

    public void Move(float distanceToMove)
    {
        background.position += Vector3.right * (distanceToMove * parallaxMultiplier); //new Vector3(distanceToMove * parallaxMultiplier, 0);
    }

    public void LoopBackground(float cameraLeftEdge, float cameraRightEdge)
    { 
        float imageRightEdge = (background.position.x + imageHalfWidth) - imageWidthOffset;
        float imageLeftEdge = (background.position.x - imageHalfWidth) + imageWidthOffset;

        if(imageRightEdge < cameraLeftEdge)
        {
            background.position += Vector3.right * imageFullWidth; //new Vector3(imageFullWidth, 0);
        }
        else if(imageLeftEdge > cameraRightEdge)
        {
            background.position += Vector3.left * -imageFullWidth; //new Vector3(-imageFullWidth, 0);
        }
    }
}
