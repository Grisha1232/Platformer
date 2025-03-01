using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class GameManager : MonoBehaviour
{

#region Variables for Pause Menu

    private GameObject pauseMenu;
    private int currentElementMenu = 0;

#endregion

    public static GameManager instance;

    [HideInInspector] public string sceneName;

    [HideInInspector] public GameState currentGameState;

    private void Awake() {
        if ( instance == null ) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        currentGameState = new GameState();
        sceneName = SceneManager.GetActiveScene().name;
    }

    void Update() {
        if (sceneName != SceneManager.GetActiveScene().name) {
            sceneName = SceneManager.GetActiveScene().name;
            LoadGame();
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

    private void OnEnable() {
        LoadGame();
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
        }
        
        if (pauseMenu.activeInHierarchy) {
            UserInput.instance.controls.Jumping.Disable();
        } else {
            UserInput.instance.controls.Jumping.Enable();
        }
    }

    public void LoadMainMenu() {
        SaveGame();
        SceneManager.LoadScene("Main Menu");
    }


#endregion

#region GameManagment

    public void ReturnToCheckpoint() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GetComponent<PlayerMovement>().UnblockMovement();
    }

    public void SaveGame() {
        BinaryFormatter formatter = new BinaryFormatter();
        
        using (FileStream stream = new FileStream("save.dat", FileMode.Create)) {
            formatter.Serialize(stream, currentGameState);
        }
        print("Game saved in file");
    }

    public void LoadGame() {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream("save.dat", FileMode.Open)) {
            currentGameState = (GameState)formatter.Deserialize(stream);
        }
        print(currentGameState);
    }

#endregion
}
