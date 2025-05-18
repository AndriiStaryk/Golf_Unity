using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject aboutPanel;
    
    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button aboutButton;
    public Button quitButton;
    
    [Header("About Panel")]
    public Button backButton;
    public TextMeshProUGUI studentNameText;
    public TextMeshProUGUI groupText;
    
    [Header("Student Information")]
    public string studentName = "Your Name Here";
    public string studentGroup = "Your Group Here";
    
    private void Start()
    {
        // Initialize the menu
        ShowMainMenu();
        
        // Set up button listeners
        SetupButtonListeners();
        
        // Set student information in about panel
        SetupAboutPanel();
    }
    
    private void SetupButtonListeners()
    {
        // Main menu buttons
        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);
            
        if (aboutButton != null)
            aboutButton.onClick.AddListener(ShowAbout);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
            
        // About panel button
        if (backButton != null)
            backButton.onClick.AddListener(ShowMainMenu);
    }
    
    private void SetupAboutPanel()
    {
        if (studentNameText != null)
            studentNameText.text = "Made by student: " + studentName;
            
        if (groupText != null)
            groupText.text = "Group: " + studentGroup;
    }
    
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    
    public void ShowAbout()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
            
        if (aboutPanel != null)
            aboutPanel.SetActive(true);
    }
    
    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
            
        if (aboutPanel != null)
            aboutPanel.SetActive(false);
    }
    
    public void QuitGame()
    {
        // Quit the application
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}