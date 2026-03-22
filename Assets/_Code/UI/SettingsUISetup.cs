using _Code.MainGame.Save;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _Code.UI
{
    public class SettingsUISetup : MonoBehaviour
    {
        private const string MasterVolumeParam = "MasterVolume";
        private const string MusicVolumeParam = "MusicVolume";
        private const string EffectsVolumeParam = "EffectsVolume";
        
        public AudioMixer audioMixer;
        public Slider masterSlider;
        public Slider musicSlider;
        public Slider effectsSlider;

        public Toggle minimapToggle;
        public GameObject minimap;

        void Start()
        {
            ResyncValues();
            
            // AUTOMATIC EVENT SUBSCRIPTIONS
            if (masterSlider) masterSlider.onValueChanged.AddListener(SetMasterVolume);
            if (musicSlider) musicSlider.onValueChanged.AddListener(SetMusicVolume);
            if (effectsSlider) effectsSlider.onValueChanged.AddListener(SetEffectsVolume);
            if (minimapToggle) minimapToggle.onValueChanged.AddListener(ToggleMinimap);
        }

        public void ResyncValues()
        {
            SaveData data = SaveManager.Load();

            // Load mixer values
            audioMixer.SetFloat(MasterVolumeParam, data.MasterVolume);
            audioMixer.SetFloat(MusicVolumeParam, data.MusicVolume);
            audioMixer.SetFloat(EffectsVolumeParam, data.EffectsVolume);
            if (minimap) minimap.SetActive(data.MinimapOpened);
            
            // Set slider values
            if (masterSlider) masterSlider.value = data.MasterVolume;
            if (musicSlider) musicSlider.value = data.MusicVolume;
            if (effectsSlider) effectsSlider.value = data.EffectsVolume;
            if (minimapToggle) minimapToggle.isOn = data.MinimapOpened;
        }


        private void ToggleMinimap(bool value)
        {
            minimap.SetActive(value);
            SaveManager.Load().MinimapOpened = minimap.activeSelf;
            SaveManager.Save(SaveManager.Load());
        }

        private void SetMasterVolume(float value)
        {
            audioMixer.SetFloat(MasterVolumeParam, value);
            SaveManager.Load().MasterVolume = value;
            SaveManager.Save(SaveManager.Load());
        }

        private void SetMusicVolume(float value)
        {
            audioMixer.SetFloat(MusicVolumeParam, value);
            SaveManager.Load().MusicVolume = value;
            SaveManager.Save(SaveManager.Load());
        }

        private void SetEffectsVolume(float value)
        {
            audioMixer.SetFloat(EffectsVolumeParam, value);
            SaveManager.Load().EffectsVolume = value;
            SaveManager.Save(SaveManager.Load());
        }

    }
}