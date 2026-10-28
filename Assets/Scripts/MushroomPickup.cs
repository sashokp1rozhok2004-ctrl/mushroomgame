using UnityEngine;
public class MushroomPickup : MonoBehaviour
{
    public void DisableForCollection()
    {
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
    }
}
