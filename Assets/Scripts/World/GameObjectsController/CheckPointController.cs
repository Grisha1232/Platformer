using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CheckPointController : MonoBehaviour
{
    public GameObject canvasTip;
    public TMP_Text text;

    public bool isTravel;
    public string NextLevel;
    public Transform TravelTo;

    private InputAction interactAction;

    private GameObject bonefireUI;
    private GameObject player;

    void Awake() {
        interactAction = UserInput.instance.controls.GameInteraction.Interact;
    }

    private void Update() {
        OpenBonefireUI();
        TravelToNextLevel();
        if (text.text != GetButtonNameForAction(interactAction)) {
            text.SetText(GetButtonNameForAction(interactAction));
        }
    }

    private void OpenBonefireUI() {
        if (bonefireUI == null) {
            bonefireUI = GameManager.FindInactiveObjectByTag("BonefireUI");
        }
        
        if (UserInput.instance.controls.GameInteraction.Interact.WasPressedThisFrame() && canvasTip.activeInHierarchy) {
            GameManager.instance.SetCheckpoint(gameObject.transform.position);
            if (!isTravel) {
                UserInput.instance.DisableForGame();
                bonefireUI.SetActive(true);
                GameManager.instance.SetSleep(true);
            }
        }
        
        if (UserInput.instance.controls.UIinteractive.Back.WasPressedThisFrame() && canvasTip.activeInHierarchy && !isTravel) {
            UserInput.instance.EnableForGame();
            bonefireUI.SetActive(false);
                GameManager.instance.SetSleep(false);
        }
    }

    private void TravelToNextLevel() {
        if (!isTravel) {
            return;
        }
        if (UserInput.instance.controls.GameInteraction.Interact.WasPressedThisFrame() && isTravel && NextLevel != "") {
            GameManager.instance.LoadScene(NextLevel);
        }
        if (UserInput.instance.controls.GameInteraction.Interact.WasPressedThisFrame() && isTravel && TravelTo != null) {
            player.GetComponent<PlayerMovement>().TeleportTo(TravelTo);
        }

    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            canvasTip.SetActive(true);
            player = collision.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collision){
        if (collision.gameObject.tag == "Player") {
            canvasTip.SetActive(false);
        }
    }

    // Метод для получения имени кнопки, связанной с действием
    private string GetButtonNameForAction(InputAction action)
    {
        // Получаем активное устройство
        InputDevice activeDevice = GetActiveDevice();

        // Перебираем все привязки (bindings) для действия
        foreach (InputBinding binding in action.bindings)
        {
            // Получаем устройство, связанное с привязкой
            InputControl control = action.controls.FirstOrDefault(c => c.device == activeDevice);

            if (control != null)
            {
                return control.displayName; // Возвращаем имя кнопки
            }
        }

        return "Не назначено";
    }

    // Метод для определения активного устройства
    private InputDevice GetActiveDevice()
    {
        // Получаем последнее использованное устройство
        InputDevice lastUsedDevice = InputSystem.devices.LastOrDefault(d => d.lastUpdateTime == InputSystem.devices.Max(d2 => d2.lastUpdateTime));

        // Если устройство — клавиатура или геймпад, возвращаем его
        if (lastUsedDevice is Keyboard || lastUsedDevice is Gamepad)
        {
            return lastUsedDevice;
        }

        // По умолчанию возвращаем клавиатуру
        return Keyboard.current;
    }

}
