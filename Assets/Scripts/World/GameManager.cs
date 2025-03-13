using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
public class GameManager : MonoBehaviour
{

#region Variables for Pause Menu

    private GameObject pauseMenu;

#endregion

    public static GameManager instance;

    [HideInInspector] public string sceneName;

    [HideInInspector] public GameState currentGameState;

    public GameObject bossUI;
    public Slider bossHealthBar;
    public TMP_Text bossName;

    public GameObject player;
    public GameObject playerModel;

    private void Awake() {
        if ( instance == null ) {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(player);
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        // TODO: УБРАТЬ!!!!!!!
        PlayerPrefs.SetInt("TrainingBossDead", 0);
        
        currentGameState = new GameState();
        sceneName = SceneManager.GetActiveScene().name;
    }

    void Update() {
        if (sceneName != SceneManager.GetActiveScene().name) {
            sceneName = SceneManager.GetActiveScene().name;
        }
        openPauseMenu();
    }

    public static GameObject FindInactiveObjectByTag(string tag)
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
                UserInput.instance.DisableForGame();
            } else {
                UserInput.instance.EnableForGame();
            }
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
            UserInput.instance.EnableForGame();
            SceneManager.LoadScene(name);
        }
        
    }

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
        playerModel.GetComponent<PlayerMovement>().UnblockMovement();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (isDeath) {
            bossUI.SetActive(false);
            foreach (var boss in FindObjectsOfType<DefaultBoss>(true)) {
                boss.Reset();
            }
        }
        playerModel.GetComponent<PlayerHealth>().Reset();
        PlayerInventory.instance.ResetHealPotions();
        UserInput.instance.EnableForGame();
    }

    public void SetCheckpoint(Vector2 position) {
        currentGameState.checkpoints[sceneName] = (position.x, position.y);
        SaveGame();
        LoadGame(true);
    }

    public void SaveGame() {
        currentGameState.currency = PlayerInventory.instance.Currency;
        if (!bossUI.activeInHierarchy) {
            currentGameState.playerPositions[sceneName] = (playerModel.transform.position.x, playerModel.transform.position.y);
        }
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
                playerModel.GetComponent<Rigidbody2D>().position = new Vector3(currentGameState.checkpoints[SceneManager.GetActiveScene().name].x, currentGameState.checkpoints[SceneManager.GetActiveScene().name].y); 
                currentGameState.playerPositions[SceneManager.GetActiveScene().name] = currentGameState.checkpoints[SceneManager.GetActiveScene().name];
            } else {
                playerModel.GetComponent<Rigidbody2D>().position = new Vector3(0, 0);
                currentGameState.checkpoints[SceneManager.GetActiveScene().name] = (0, 0);
                currentGameState.playerPositions[SceneManager.GetActiveScene().name] = currentGameState.checkpoints[SceneManager.GetActiveScene().name];
            }
        } else {
            if (currentGameState.playerPositions.ContainsKey(SceneManager.GetActiveScene().name)) {
                playerModel.GetComponent<Rigidbody2D>().position = new Vector3(currentGameState.playerPositions[SceneManager.GetActiveScene().name].x, currentGameState.playerPositions[SceneManager.GetActiveScene().name].y); 
            } else {
                playerModel.GetComponent<Rigidbody2D>().position = new Vector3(0, 0);
                currentGameState.checkpoints[SceneManager.GetActiveScene().name] = (0, 0);
                currentGameState.playerPositions[SceneManager.GetActiveScene().name] = currentGameState.checkpoints[SceneManager.GetActiveScene().name];
            }
        }
        PlayerMovement.isMovementBlocked = false;
        PlayerInventory.instance.Currency = currentGameState.currency;
        PlayerInventory.instance.AddToCurrentCurency(0);
    }

    public void SetBossHealth(EnemyHealth health, string name) {
        bossUI.SetActive(true);
        health.healthBar = bossHealthBar;
        bossName.text = name;
    }

    public void BossDied() {
        bossUI.SetActive(false);
    }

#endregion
}
