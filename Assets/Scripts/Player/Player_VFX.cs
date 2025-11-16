using System.Collections;
using UnityEngine;

public class Player_VFX : Entity_VFX
{
    [Header("Image Echo VFX")]
    [Range(0.01f, .2f)] 
    [SerializeField] public float imageEchoInterval = 0.05f;
    [SerializeField] private GameObject imageEchoPrefab;
    private Coroutine imageEchoCo;

    public void DoImageEchoEffect(float duration)
    {
        if(imageEchoCo != null)
            StopCoroutine(imageEchoCo);
        imageEchoCo = StartCoroutine(ImageEchoEffectCo(duration));
    }
    private IEnumerator ImageEchoEffectCo(float duration)
    {

        float timeTracker = 0f;

        while (timeTracker < duration)
        {
            CreateImageEcho();
            yield return new WaitForSeconds(imageEchoInterval);
            timeTracker += imageEchoInterval;
        }
    }

    private void CreateImageEcho()
    {
        GameObject imageEcho = Instantiate(imageEchoPrefab, transform.position, transform.rotation);
        imageEcho.GetComponentsInChildren<SpriteRenderer>()[0].sprite = sr.sprite;
    }

}
