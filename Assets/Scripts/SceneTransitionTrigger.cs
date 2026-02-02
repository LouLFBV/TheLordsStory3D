using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        //other.GetComponent<MoveBehaviour>().canMove = false;
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        anim.SetTrigger("Start");
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneToLoad);
    }
}