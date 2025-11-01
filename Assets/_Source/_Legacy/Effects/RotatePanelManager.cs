using UnityEngine;

public class ShowInPortrait : MonoBehaviour
{
    [Tooltip("Объект, который нужно показывать/скрывать")]
    public GameObject targetObject;

    private void Update()
    {
        bool isPortrait = Screen.height > Screen.width;

        if (targetObject != null)
        {
            targetObject.SetActive(isPortrait);
        }
    }
}