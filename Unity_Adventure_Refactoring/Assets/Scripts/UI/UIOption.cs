using UnityEngine;

public class UIOption : MonoBehaviour
{
    [SerializeField] GameObject settingScreen;
    private bool isSetOn = false;
    void Start()
    {
        CharacterManager.Instance.Player.controller.onSettingScreen += SettingOpen;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleSetOn()
    {
        isSetOn = !isSetOn;
    }

    void SettingOpen()
    {
        ToggleSetOn();
        settingScreen.SetActive(isSetOn);
    }
}
