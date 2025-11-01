using UnityEngine;
using System.Collections;

public class ProjectileMover : MonoBehaviour
{
    public int moveType = 0;
    public Vector2 targetPosition;
    public float speed = 10f;
    public float rotationSpeed = 100f;
    public GameObject[] spawnPrefabs;
    private bool spawned = false;
    public float destroyDelay = 1f;
    private bool canMove = false;

    public void Move(ProjectileTarget startTarget, ProjectileTarget endTarget)
    {
        switch(moveType)
        {
            case 0: 
                transform.position = startTarget.gameObject.transform.position;
                targetPosition = endTarget.gameObject.transform.position;
                break;
            case 1: 
                targetPosition = endTarget.gameObject.transform.position + endTarget.offset1;
                transform.position = targetPosition + Vector2.down * 3;
                break;
        }
        GetComponent<SpriteRenderer>().enabled = true;
                    
        canMove = true;
    }

    private void Update()
    {
        if(canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            if ((Vector2)transform.position == targetPosition)
            {
                if (!spawned && spawnPrefabs.Length > 0)
                {
                    foreach(GameObject go in spawnPrefabs)
                        Instantiate(go, targetPosition, Quaternion.identity, transform.parent);
                    GetComponent<SpriteRenderer>().enabled = false;
                    spawned = true;
                }

                StartCoroutine(DestroyAfterDelay());
            }
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}