using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EndPanel : MonoBehaviour
{
    public VisualElement retryButton;

    public VisualElement giveUpButton;

    public Label scoreDisplay;

    public PlayerController player;
    // Start is called before the first frame update
    void Start() //Initialisation, get objects from ui document and assign them a value or an event (for buttons)
    {
        scoreDisplay = GetComponent<UIDocument>().rootVisualElement.Q<Label>("scoreDisplay");
        scoreDisplay.text = player.score.ToString();
            
        retryButton = GetComponent<UIDocument>().rootVisualElement.Q("retryButton");
        retryButton.RegisterCallback<ClickEvent>(OnClickRetry);
        
        giveUpButton = GetComponent<UIDocument>().rootVisualElement.Q("giveUpButton");
        giveUpButton.RegisterCallback<ClickEvent>(OnClickGiveUp);
    }

    private void OnClickRetry(ClickEvent evt) //When we click on retry, launch the scene again
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void OnClickGiveUp(ClickEvent evt) //Close the game on quit
    {
        UnityEditor.EditorApplication.isPlaying = false; //Close play mode in editor
        Application.Quit(); //Close application (only in build)
    }
}
