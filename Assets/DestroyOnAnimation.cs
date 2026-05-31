using UnityEngine;

public class DestroyOnAnimationEnd : MonoBehaviour
{
    void Start()
    {
        // Mengambil durasi total clip animasi yang menempel pada objek ini
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            
            // Hancurkan objek efek ini secara otomatis begitu durasi animasinya habis
            Destroy(gameObject, animationLength);
        }
        else
        {
            // Pengaman jika Animator tidak ketemu, hancurkan dalam 0.5 detik
            Destroy(gameObject, 0.5f);
        }
    }
}