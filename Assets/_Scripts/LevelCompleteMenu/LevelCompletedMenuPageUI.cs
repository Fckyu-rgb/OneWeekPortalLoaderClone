using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.LevelCompletedMenu.UI
{
    public class LevelCompletedMenuPageUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _page;

        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _backToStartMenuButton;

        public event Action OnNextLevelButtonClicked;

        public event Action OnBackToStartMenuClicked;

        private void Awake()
        {
            _nextLevelButton.onClick.AddListener(() => OnNextLevelButtonClicked?.Invoke());
            _backToStartMenuButton.onClick.AddListener(() => OnBackToStartMenuClicked?.Invoke());

            if (IsOpen())
                HideMenu();
        }

        public bool IsOpen()
        {
            return _page.gameObject.activeSelf;
        }

        public void OpenMenu()
        {
            _page.gameObject.SetActive(true);
        }

        public void HideMenu()
        {
            _page.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _nextLevelButton.onClick.RemoveAllListeners();
            _backToStartMenuButton.onClick.RemoveAllListeners();
        }
    }
}