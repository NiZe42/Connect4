using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI mainMessageText;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Debug.LogWarning("UIManager already exists in the scene. Deleting this instance.");
            Destroy(this);
        }
    }

    public void DisplayMessage(string message) {
        StopAllCoroutines();
        mainMessageText.text = message;
    }

    public void DisplayMessageForTime(string message, float time) {
        StopAllCoroutines();
        StartCoroutine(MessageDisplayCouroutine(message, time));
    }

    private IEnumerator MessageDisplayCouroutine(string message, float time) {
        string previousText = mainMessageText.text;
        mainMessageText.text = message;
        yield return new WaitForSeconds(time);
        mainMessageText.text = previousText;
    }
}
