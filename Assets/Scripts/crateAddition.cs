using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crateAddition : MonoBehaviour
{
    public GameObject cratePrefab;
    float xMin = -11, xMax = 11, yMin = -4, yMax = 3;
    private List<GameObject> currentCollisions = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            float crateX = this.gameObject.transform.position.x + Random.Range(xMin, xMax);
            float crateY = this.gameObject.transform.position.y + Random.Range(yMin, yMax);
            Instantiate(cratePrefab, new Vector3(crateX, crateY, 1), Quaternion.identity);
        }
    }

    void Update()
    {
        if (currentCollisions.Count < 10)
        {
            float crateX = this.gameObject.transform.position.x + Random.Range(xMin, xMax);
            float crateY = this.gameObject.transform.position.y + Random.Range(yMin, yMax);
            Instantiate(cratePrefab, new Vector3(crateX, crateY, 1), Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Crate")
        {
            currentCollisions.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentCollisions.Remove(collision.gameObject);
    }
}
