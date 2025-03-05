using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class GameManager : MonoBehaviour
{

#region Variables for Pause Menu

    private GameObject pauseMenu;

#endregion

    public static GameManager instance;

    [HideInInspector] public string sceneName;

    [HideInInspector] public GameState currentGameState;

    public GameObject player;

    private void Awake() {
        if ( instance == null ) {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(player);
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        currentGameState = new GameState();
        for (int i = 0; i < 10; i++) {
            currentGameState.Items.Add(new Item(i.ToString()));
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

    private void OnEnable() {
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
        LoadScene("Main Menu");
    }


#endregion

#region GameManagment

    public void LoadScene(string name) {
        if (name == "Main Menu") {
            SaveGame();
            player.SetActive(false);
            SceneManager.LoadScene(name);
        } else {
            player.SetActive(true);
            SceneManager.LoadScene(name);
        }
        
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        if (scene.name != "Main Menu") {
            Pathfinder.instance.setMap();
            LoadGame(false);
        } else {
            player.SetActive(false);
        }
    }

    public void ReturnToCheckpoint(bool isDeath = false) {
        LoadGame(isDeath);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GetComponent<PlayerMovement>().UnblockMovement();
    }

    public void SaveGame() {
        currentGameState.currency = PlayerInventory.instance.Currency;
        currentGameState.Items = PlayerInventory.instance.Items;
        currentGameState.QuickItems = PlayerInventory.instance.quickItems;
        currentGameState.playerPositions[sceneName] = (player.transform.position.x, player.transform.position.y);
        BinaryFormatter formatter = new BinaryFormatter();
        
        using (FileStream stream = new FileStream("save.dat", FileMode.Create)) {
            formatter.Serialize(stream, currentGameState);
        }
        print("Game saved in file");
    }

    public void LoadGame(bool useCheckpoint) {
        BinaryFormatter formatter = new BinaryFormatter();
        if (File.Exists("save.dat")) {
            using (FileStream stream = new FileStream("save.dat", FileMode.Open)) {
                currentGameState = (GameState)formatter.Deserialize(stream);
            }
        }
        if (currentGameState == null) {
            currentGameState = new();
        }
        if (useCheckpoint) {
            currentGameState.countDeath++;
            if (currentGameState.checkpoints.ContainsKey(SceneManager.GetActiveScene().name)) {
                player.GetComponent<Rigidbody2D>().position = new Vector3(currentGameState.checkpoints[SceneManager.GetActiveScene().name].x, currentGameState.checkpoints[SceneManager.GetActiveScene().name].y); 
                currentGameState.playerPositions[SceneManager.GetActiveScene().name] = currentGameState.checkpoints[SceneManager.GetActiveScene().name];
            } else {
                player.GetComponent<Rigidbody2D>().position = new Vector3(0, 10);
                currentGameState.checkpoints[SceneManager.GetActiveScene().name] = (0, 10);
                currentGameState.playerPositions[SceneManager.GetActiveScene().name] = currentGameState.checkpoints[SceneManager.GetActiveScene().name];
            }
        } else {
            if (currentGameState.playerPositions.ContainsKey(SceneManager.GetActiveScene().name)) {
                player.GetComponent<Rigidbody2D>().position = new Vector3(currentGameState.playerPositions[SceneManager.GetActiveScene().name].x, currentGameState.playerPositions[SceneManager.GetActiveScene().name].y); 
                currentGameState.checkpoints[SceneManager.GetActiveScene().name] = currentGameState.playerPositions[SceneManager.GetActiveScene().name];
            } else {
                player.GetComponent<Rigidbody2D>().position = new Vector3(0, 10);
                currentGameState.checkpoints[SceneManager.GetActiveScene().name] = (0, 10);
                currentGameState.playerPositions[SceneManager.GetActiveScene().name] = currentGameState.checkpoints[SceneManager.GetActiveScene().name];
            }
        }
        PlayerMovement.isMovementBlocked = false;
        PlayerInventory.instance.Currency = currentGameState.currency;
        PlayerInventory.instance.Items = currentGameState.Items;
        PlayerInventory.instance.quickItems = currentGameState.QuickItems;
    }

#endregion
}
