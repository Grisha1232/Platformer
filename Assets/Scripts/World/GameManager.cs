using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

#region Variables for Pause Menu

    private GameObject pauseMenu;
    private int currentElementMenu = 0;

#endregion

    public static GameManager instance;

    [HideInInspector] public string sceneName;

    private void Awake() {
        if ( instance == null ) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        sceneName = SceneManager.GetActiveScene().name;
    }

    void Update() {
        if (sceneName != SceneManager.GetActiveScene().name) {
            sceneName = SceneManager.GetActiveScene().name;
        }
        openPauseMenu();
    }

    private GameObject FindInactiveObjectByTag(string tag)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag(tag))
            {
                return obj;
            }
        }
        return null;
    }


#region Pause Menu

    private void openPauseMenu() {
        if (sceneName == "Main Menu") {
            return;
        }
        if (pauseMenu == null) {
            pauseMenu = FindInactiveObjectByTag("PauseMenu");
        }
        if (UserInput.instance.controls.UIinteractive.PauseMenu.WasPressedThisFrame()) {
            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
            if (pauseMenu.activeInHierarchy) {
                UserInput.instance.controls.Jumping.Disable();
            } else {
                UserInput.instance.controls.Jumping.Enable();
            }
        }
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene("Main Menu");
    }


#endregion
}
