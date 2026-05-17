using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
   public void PindahKeMainMenu()
    {
        Debug.Log("Pindah Ke Main Menu");
        SceneManager.LoadScene("MainMenu");
    }
}
