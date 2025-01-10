using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace GD3D
{
    public class TatPanel : MonoBehaviour
    {
        // Start is called before the first frame update
        public void LoadSceneByName()
        {
            SceneManager.LoadScene("Menu");
        }

        // Optional: Method to load a scene by its index
        public void LoadSceneByIndex()
        {
            SceneManager.LoadScene(0);
        }
    }
}
