using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int fruitLevel = 1;
    public GameObject nextLevelPrefab;
    private bool hasMerged = false;

    void OnCollisionEnter(Collision collision)
    {
        if (hasMerged) return;

        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();

        if (otherFruit != null && otherFruit.fruitLevel == this.fruitLevel && !otherFruit.hasMerged)
        {
            this.hasMerged = true;
            otherFruit.hasMerged = true;

            if (nextLevelPrefab != null)
            {
                Instantiate(nextLevelPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
            Destroy(otherFruit.gameObject);
        }
    }
}