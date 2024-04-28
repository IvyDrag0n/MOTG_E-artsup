using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RetryPanel : MonoBehaviour
{
    public VisualElement retryButton;

    public VisualElement giveUpButton;

    private VisualElement focusedButton;

    private Vector2 delta; //for mouse movement
    
    
    // Start is called before the first frame update
    void Start() //Initialisation, get objects from ui document and assign them an event
    {
        retryButton = GetComponent<UIDocument>().rootVisualElement.Q("retryButton");
        retryButton.RegisterCallback<ClickEvent>(OnClickRetry);
        
        giveUpButton = GetComponent<UIDocument>().rootVisualElement.Q("giveUpButton");
        giveUpButton.RegisterCallback<ClickEvent>(OnClickGiveUp);

        focusedButton = retryButton;
        retryButton.Focus();
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

    public void NavigationUp(InputAction.CallbackContext context) //When we press up or use the joystick / dpad
    {
        if (context.performed) //Switch between the two buttons
        {
            if (focusedButton == retryButton) 
            {
                focusedButton = giveUpButton;
                giveUpButton.Focus(); //highlight the button
            }

            else if (focusedButton == giveUpButton)
            {
                focusedButton = retryButton;
                retryButton.Focus(); //highlight the button
            }
            
            else if (focusedButton == null) //If none is focused, default to retry
            {
                retryButton.Focus();
                focusedButton = retryButton;
            }
        }
        
    }
    
    public void NavigationDown(InputAction.CallbackContext context) //When we press down or use the joystick / dpad
    {
        
        if (context.performed) //Switch between the two buttons
        {
            if (focusedButton == retryButton)
            {
                focusedButton = giveUpButton;
                giveUpButton.Focus();//highlight the button
                
            }

            else if (focusedButton == giveUpButton)
            {
                focusedButton = retryButton;
                retryButton.Focus(); //highlight the button
            }
            
            else if (focusedButton == null) //If none is focused, default to retry
            {
                retryButton.Focus();
                focusedButton = retryButton;
            }
        }
    }
    
    public void NavigationSelect(InputAction.CallbackContext context) //launch corresponding events
    {
        if (context.started)
        {
            if (focusedButton == retryButton)
            {
                OnClickRetry(new ClickEvent());
            }

            else if (focusedButton == giveUpButton)
            {
                OnClickGiveUp(new ClickEvent());
            }
        }
    }

    public void MouseMovement(InputAction.CallbackContext context) //detect mouse movement to reset focus
    {
        delta = context.ReadValue<Vector2>();
        if (delta != new Vector2())
        {
            retryButton.Blur();
            giveUpButton.Blur();
            focusedButton = null;
        }
        
    }
}
