using UnityEngine;

public class KeepSpriteUpright : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}