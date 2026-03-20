using _Code.MainGame.Save;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _Code.UI
{
    public class AudioSettingsUI : MonoBehaviour
    {
        private const string MasterVolumeParam = "MasterVolume";
        private const string MusicVolumeParam = "MusicVolume";
        private const string EffectsVolumeParam = "EffectsVolume";
        
        public AudioMixer audioMixer;
        public Slider masterSlider;
        public Slider musicSlider;
        public Slider effectsSlider;



        void Start()
        {
            var data = SaveManager.Load();

            // Load mixer values
            audioMixer.SetFloat(MasterVolumeParam, data.MasterVolume);
            audioMixer.SetFloat(MusicVolumeParam, data.MusicVolume);
            audioMixer.SetFloat(EffectsVolumeParam, data.EffectsVolume);

            // Set slider values
            masterSlider.value = data.MasterVolume;
            musicSlider.value = data.MusicVolume;
            effectsSlider.value = data.EffectsVolume;

            // AUTOMATIC EVENT SUBSCRIPTIONS
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            effectsSlider.onValueChanged.AddListener(SetEffectsVolume);
        }




        public void SetMasterVolume(float value)
        {
            audioMixer.SetFloat(MasterVolumeParam, value);
            SaveManager.Load().MasterVolume = value;
            SaveManager.Save(SaveManager.Load());
        }

        public void SetMusicVolume(float value)
        {
            audioMixer.SetFloat(MusicVolumeParam, value);
            SaveManager.Load().MusicVolume = value;
            SaveManager.Save(SaveManager.Load());
        }

        public void SetEffectsVolume(float value)
        {
            audioMixer.SetFloat(EffectsVolumeParam, value);
            SaveManager.Load().EffectsVolume = value;
            SaveManager.Save(SaveManager.Load());
        }

    }
}