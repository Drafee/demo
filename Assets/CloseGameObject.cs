using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseGameObject : MonoBehaviour
{
    public void DisableSelf() {
        gameObject.SetActive(false);
    }
}
