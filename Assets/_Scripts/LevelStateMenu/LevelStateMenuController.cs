using Core.GameEventSystem;
using Core.GameEventSystem.Signals;
using Core.LevelStateMenu.UI;
using SceneFieldTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core.LevelStateMenu.Controller
{
    public class LevelStateMenuController : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas = null;

        [SerializeField] private LevelStateCompletedPageUI _levelCompletedPage = null;
        [SerializeField] private LevelStateFailurePageUI _levelFailurePage = null;

        [SerializeField] private SceneField _mainMenuScene = null;

        [SerializeField] private EventBus _eventBus = null;

        [Inject]
        public void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Awake()
        {
            _canvas.worldCamera = Camera.main;

            _eventBus.Subscribe<LevelCompletedSignal>(OpenCompletedPage);
            _eventBus.Subscribe<LevelFailureSignal>(OpenFailurePage);

            _levelCompletedPage.OnNextLevelButtonClicked += StartNextLevel;
            _levelCompletedPage.OnBackToStartMenuClicked += BackToStartMenu;

            _levelFailurePage.OnRestartLevelButtonClicked += ResetCurrentLevel;
            _levelFailurePage.OnBackToStartMenuClicked += BackToStartMenu;
        }

        private void Start()
        {
            if (_levelCompletedPage.IsOpen())
                _levelCompletedPage.HideMenu();

            if (_levelFailurePage.IsOpen())
                _levelFailurePage.HideMenu();
        }

        private void ResetCurrentLevel()
        {
            _eventBus.Invoke(new CloseUISignal(_levelFailurePage.transform));
            _levelFailurePage?.HideMenu();

            _eventBus.Invoke(new LevelResetSignal());
        }

        private void StartNextLevel()
        {
            _eventBus.Invoke(new CloseUISignal(_levelCompletedPage.transform));
            _levelCompletedPage.HideMenu();

            _eventBus.Invoke(new LevelLoadNextSignal());
        }

        private void OpenCompletedPage(LevelCompletedSignal signal)
        {
            _levelCompletedPage.OpenMenu();
            _eventBus.Invoke(new OpenUISignal(_levelCompletedPage.transform));
        }

        private void OpenFailurePage(LevelFailureSignal signal)
        {
            _levelFailurePage.OpenMenu();
            _eventBus.Invoke(new OpenUISignal(_levelFailurePage.transform));
        }

        private void BackToStartMenu()
        {
            _eventBus.Invoke(new OpenUISignal(null));
            SceneManager.LoadScene(_mainMenuScene);
        }

        private void OnDestroy()
        {
            _eventBus.Unsubscribe<LevelCompletedSignal>(OpenCompletedPage);
            _eventBus.Unsubscribe<LevelFailureSignal>(OpenFailurePage);

            _levelCompletedPage.OnNextLevelButtonClicked -= StartNextLevel;
            _levelCompletedPage.OnBackToStartMenuClicked -= BackToStartMenu;

            _levelFailurePage.OnRestartLevelButtonClicked -= ResetCurrentLevel;
            _levelFailurePage.OnBackToStartMenuClicked -= BackToStartMenu;
        }
    }
}