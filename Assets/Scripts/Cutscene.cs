using System;
using UnityEngine;

public class IntroCutscenePlayer : MonoBehaviour, ICutscenePlayer
{
    [SerializeField] private Animator animator;
    [SerializeField] private float cutsceneDuration = 194f; // Länge deiner Animation in Sekunden

    public void PlayIntro(Action onFinished)
    {
        animator.Play("Cutscene"); // Name deines Animation-Clips
        StartCoroutine(WaitAndFinish(onFinished));
    }

    private System.Collections.IEnumerator WaitAndFinish(Action onFinished)
    {
        yield return new WaitForSeconds(cutsceneDuration);
        onFinished?.Invoke();
    }
}