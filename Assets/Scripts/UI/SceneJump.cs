using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneJump : MonoBehaviour
{
    public void JumpToIntro() {
        SceneManager.LoadScene(1);
    }
}
