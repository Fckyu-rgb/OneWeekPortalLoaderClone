using Core.GameEventSystem;
using Core.GameEventSystem.Signals;
using Core.Utility.DebugTool;
using UnityEngine;
using Zenject;

namespace Core.Gameplay
{
    public class BatteryChargeController : MonoBehaviour
    {
        [SerializeField] private int _batteryCharges = 0;

        [SerializeField] private int _maximumBatteryCharges = 10;

        [SerializeField] private int _playerMoveCost = 1;

        [SerializeField] private EventBus _eventBus = null;

        [SerializeField] private bool _isBatteeryLifted = false;

        [SerializeField] private bool _lockFailure = false;

        [SerializeField] private DebugLogger _debuger = new();

        [Inject]
        public void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Awake()
        {
            _batteryCharges = _maximumBatteryCharges;

            _eventBus.Subscribe<BatteryLiftSignal>(OnBatteryLift);
            _eventBus.Subscribe<LevelCompletedSignal>(LockFailure);
            _eventBus.Subscribe<AccumulatorLiftSignal>(ChargeBattery);
        }

        private void ChargeBattery(AccumulatorLiftSignal signal)
        {
            ChangeBatteryCharge(signal.Charge);
        }

        private void ChangeBatteryCharge(int toAdd)
        {
            _batteryCharges += toAdd;
            if (_batteryCharges > _maximumBatteryCharges)
            {
                _batteryCharges = _maximumBatteryCharges;
            }
            _eventBus.Invoke(new BatteryChargeChangedSignal(_batteryCharges));
        }

        private void LockFailure(LevelCompletedSignal signal)
        {
            _lockFailure = true;
        }

        private void OnBatteryLift(BatteryLiftSignal signal)
        {
            _isBatteeryLifted = true;
            _eventBus.Invoke(new BatteryInfoSignal(_maximumBatteryCharges, _batteryCharges));
            _eventBus.Subscribe<PlayerInputMoveStartSignal>(OnPlayerMove);
            _eventBus.Subscribe<PlayerInputMoveEndSignal>(CheckBatteryCharge);
        }

        private void OnPlayerMove(PlayerInputMoveStartSignal Signal)
        {
            if (!_isBatteeryLifted) return;

            ChangeBatteryCharge(-_playerMoveCost);

            _debuger.Log(this, $"Current battery charge: {_batteryCharges}");
        }

        private void CheckBatteryCharge(PlayerInputMoveEndSignal signal)
        {
            if (_batteryCharges <= 0)
            {
                _debuger.Log(this, $"Battery have no charges!");

                if (!_lockFailure)
                {
                    _eventBus.Invoke(new GameOverSignal());
                }
            }
        }

        private void OnDestroy()
        {
            _eventBus.Unsubscribe<BatteryLiftSignal>(OnBatteryLift);
            if (_isBatteeryLifted)
            {
                _eventBus.Unsubscribe<PlayerInputMoveStartSignal>(OnPlayerMove);
                _eventBus.Unsubscribe<PlayerInputMoveEndSignal>(CheckBatteryCharge);
            }
            _eventBus.Unsubscribe<AccumulatorLiftSignal>(ChargeBattery);
        }
    }
}