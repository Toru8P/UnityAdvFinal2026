using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Code.MainGame.Input
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
            if (!asset)
                return;

            var eventSystem = EventSystem.current ?? Object.FindFirstObjectByType<EventSystem>();
            if (!eventSystem)
                return;

            var module = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (module)
                module.actionsAsset = asset;

            EnsureFirstSelected(eventSystem);
        }

        private static void EnsureFirstSelected(EventSystem eventSystem)
        {
            if (!eventSystem)
                return;

            if (eventSystem.currentSelectedGameObject != null)
                return;

            var selectables = Object.FindObjectsByType<Selectable>(FindObjectsSortMode.None);
            foreach (var s in selectables)
            {
                if (!s || !s.IsActive() || !s.interactable) continue;
                eventSystem.SetSelectedGameObject(s.gameObject);
                return;
            }
        }
    }
}
