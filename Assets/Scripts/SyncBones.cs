using UnityEngine;

public class SyncBones : MonoBehaviour
{
    public SkinnedMeshRenderer targetVisual; // Le mesh du vêtement
    public Transform rootBoneTarget; // Le Root Bone de ton personnage (souvent nommé 'Hips')

    [ContextMenu("Sync Bones Now")]
    public void Sync()
    {
        // On récupère tous les os du personnage principal
        Transform[] targetBones = rootBoneTarget.GetComponentsInChildren<Transform>();
        Transform[] meshBones = targetVisual.bones;
        Transform[] newBones = new Transform[meshBones.Length];

        // On fait correspondre chaque os du vêtement avec celui du perso
        for (int i = 0; i < meshBones.Length; i++)
        {
            foreach (var t in targetBones)
            {
                if (t.name == meshBones[i].name)
                {
                    newBones[i] = t;
                    break;
                }
            }
        }

        targetVisual.bones = newBones;
        targetVisual.rootBone = rootBoneTarget;
        Debug.Log("Bones synchronisés pour " + gameObject.name);
    }
}