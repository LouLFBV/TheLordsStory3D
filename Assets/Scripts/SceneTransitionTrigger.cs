using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public sealed class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string targetSpawnID;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private Animator anim;

    private bool _isTransitioning;

    private void OnTriggerEnter(Collider other)
    {
        if (_isTransitioning)
            return;

        if (!other.CompareTag("Player"))
            return;

        _isTransitioning = true;

        //if (other.TryGetComponent<PlayerController>(out var controller))
        //    controller.DisableInput();

        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        if (anim != null)
            anim.SetTrigger("Start");

        yield return new WaitForSeconds(delay);

        GameManager.Instance.SpawnSystem.SetSpawn(targetSpawnID);
        SceneManager.LoadSceneAsync(sceneToLoad);
    }
}