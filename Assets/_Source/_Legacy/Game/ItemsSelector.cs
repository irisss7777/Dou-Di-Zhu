using UnityEngine;

public class ItemsSelector : MonoBehaviour
{
    [SerializeField] private GameObject[] objects;

    public void Select(int objId)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
        objects[objId].SetActive(true);
    }
}
