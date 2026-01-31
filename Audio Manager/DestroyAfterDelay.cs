using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    public float lifetime;
    
    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
            Destroy(gameObject);
    }
}
