using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IngameMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private GameObject emojiSelectPanel;
    [SerializeField] private GameObject playerSelectPanel;
    [SerializeField] private EmojiManager emojiManager;

    void Start()
    {
        float volume = 0.5f;
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void OpenInfo()
    {
        infoPanel.SetActive(true);
    }

    public void OpenEmojiPanel()
    {
        emojiSelectPanel.SetActive(true);
    }

    public void OpenEmojiPlayerSelectPanel(int emojiIndex)
    {
        emojiManager.selectedEmojiId = emojiIndex;
        playerSelectPanel.SetActive(true);
        emojiSelectPanel.SetActive(false);
    }

    public void Close()
    {
        settingsPanel.SetActive(false);
        infoPanel.SetActive(false);
        playerSelectPanel.SetActive(false);
        emojiSelectPanel.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetVolume()
    {
        float volume = volumeSlider.value;
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}