using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Code.MainGame
{
    public static class InputActionsBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnSceneLoaded()
        {
            Apply();
            SceneManager.sceneLoaded += (_, __) => Apply();
        }

        private static void Apply()
        {
            var asset = Resources.Load<InputActionAsset>("InputSystem_Actions");
            if (asset == null)
                return;

            var eventSystem = EventSystem.current ?? Object.FindFirstObjectByType<EventSystem>();
            if (eventSystem == null)
                return;

            var module = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (module != null)
                module.actionsAsset = asset;

            EnsureFirstSelected(eventSystem);
        }

        private static void EnsureFirstSelected(EventSystem eventSystem)
        {
            if (eventSystem == null)
                return;

            if (eventSystem.currentSelectedGameObject != null)
                return;

            var selectables = Object.FindObjectsByType<Selectable>(FindObjectsSortMode.None);
            foreach (var s in selectables)
            {
                if (s != null && s.IsActive() && s.interactable)
                {
                    eventSystem.SetSelectedGameObject(s.gameObject);
                    return;
                }
            }
        }
    }
}
