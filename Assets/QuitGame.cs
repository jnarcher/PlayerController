using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuitGame : MonoBehaviour
{
    public void OnQuit(InputAction.CallbackContext _)
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying) EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
