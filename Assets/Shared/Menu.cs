using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shared
{
    public class Menu : MonoBehaviour
    {
 
        public GameObject dropdown;

        private void Start()
        {
            dropdown.SetActive(false);
        }

        public void PrototypeOne()
        {
            SceneManager.LoadScene("PrototypeOneMain");
        }

        public void PrototypeTwo()
        {
            SceneManager.LoadScene("PrototypeTwoMain");
        }

        public void PrototypeThree()
        {
            SceneManager.LoadScene("PrototypeThreeMain");
        }

        public void PrototypeFour()
        {
            SceneManager.LoadScene("PrototypeFourMain");
        }

        public void PrototypeFive()
        {
            SceneManager.LoadScene("PrototypeFiveMain");
        }

        public void PrototypeSix()
        {
            SceneManager.LoadScene("PrototypeSixMain");
        }

        public void PrototypeSeven()
        {
            SceneManager.LoadScene("PrototypeSevenMain");
        }
        public void MainMenu()
        {
            SceneManager.LoadScene("Main_Menu");
        }

        public void ChooseMode()
        {
            dropdown.SetActive(true);
        }

        public void ExitChooseMode()
        {
            dropdown.SetActive(false);
        }

        public void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }
    }
}
