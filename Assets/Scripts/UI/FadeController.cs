using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeController : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 2f;

    private void Start()
    {
        fadeImage.DOFade(0f, fadeDuration).OnComplete(() => gameObject.SetActive(false));
    }
}