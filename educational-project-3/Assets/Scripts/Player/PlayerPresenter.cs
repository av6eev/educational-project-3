using System.Collections;
using System.Threading.Tasks;
using Bot;
using Game;
using UnityEngine;
using Utilities;

namespace Player
{
    public class PlayerPresenter : IPresenter
    {
        private readonly PlayerModel _model;
        private readonly GameManager _manager;
        private readonly PlayerView _view;

        private Coroutine _deathAnimationCoroutine;

        public PlayerPresenter(PlayerModel model, GameManager manager, PlayerView view)
        {
            _model = model;
            _manager = manager;
            _view = view;
        }

        public void Deactivate()
        {
            _model.OnPlayerCreated -= InitPlayer;
            _model.OnPlayerRemoved -= DestroyPlayer;
            _model.OnPlayerMove -= MovePlayer;
            _model.OnPlayerAttack -= AttackEnemy;
            _model.OnDealDamage -= DamageApplied;
        }

        public void Activate()
        {
            _view.Disable();
            
            _model.OnPlayerCreated += InitPlayer;
            _model.OnPlayerRemoved += DestroyPlayer;
            _model.OnPlayerMove += MovePlayer;
            _model.OnPlayerAttack += AttackEnemy;
            _model.OnDealDamage += DamageApplied;
        }

        private void DamageApplied()
        {
            _view.UpdateHealthBar(_model.MaxHealth, _model.CurrentHealth);

            if (!(_model.CurrentHealth <= 0)) return;
            
            _view.PlayDeathAnimation();
            _deathAnimationCoroutine = GameCoroutines.RunCoroutine(GetDeathAnimationProgress());
        }

        private async void AttackEnemy()
        {
            if (_view.VisibleTargets.Count == 0) return;
            if (!_model.Id.Equals(_manager.GameModel.ActivePlayer.Id)) return;
            
            var enemy = _manager.GameModel.GetEnemyModel();
            var damage = _model.AttackDamage * ((100 - enemy.Resistance) / 100);
            
            _view.PlayAttackAnimation();
            enemy.DealDamage(damage);

            if (!(enemy.CurrentHealth >= 0)) return;
            
            await Task.Delay(1000);
            _manager.GameModel.ChangeTurn();
        }

        private async void MovePlayer(string emoji)
        {
            var cells = _manager.FloorModel.Cells;
            var transform = _view.transform;
            var forward = transform.forward;
            var right = transform.right;

            Vector3 moveDirection;
            
            switch (emoji)
            {
                case BotCommandHelper.MoveTopEmoji:
                    moveDirection = forward;
                    break;
                case BotCommandHelper.MoveBottomEmoji:
                    moveDirection = -forward;
                    break;
                case BotCommandHelper.MoveLeftEmoji:
                    moveDirection = -right;
                    break;
                case BotCommandHelper.MoveRightEmoji:
                    moveDirection = right;
                    break;
                default:
                    moveDirection = Vector3.zero;
                    break;
            }
            
            var position = _model.Position + moveDirection;
            
            cells.TryGetValue(new Vector3(position.x, 0, position.z), out var newCell);

            if (newCell is not { IsPlayable: true })
            {
                _manager.GameModel.ChangeTurn();
                return;
            }
            
            cells[new Vector3(_manager.GameModel.ActivePlayer.Position.x, 0, _manager.GameModel.ActivePlayer.Position.z)].IsActive = false;
            newCell.IsActive = true;

            _model.Direction = moveDirection;

            _view.PlayWalkAnimation(true);

            await Task.Delay(3500);            
            
            _model.Position += moveDirection;
            _model.Direction = Vector3.zero;
            
            _view.PlayWalkAnimation(false);

            _manager.GameModel.ChangeTurn();
        }

        private void InitPlayer()
        {
            var playerDescription = _manager.GameDescriptions.Players[_model.ClassType];

            _model.CurrentHealth = playerDescription.MaxHealth;
            _model.MaxHealth = playerDescription.MaxHealth;
            _model.AttackDamage = playerDescription.AttackDamage;
            _model.Resistance = playerDescription.Resistance;
            
            _view.Animator.runtimeAnimatorController = playerDescription.AnimatorOverrideController;
            _view.Text.text = _model.Name;
            
            _view.Enable();
        }
        
        private void DestroyPlayer()
        {
            _view.DestroyPlayer();
        }
           
        private IEnumerator GetDeathAnimationProgress()
        {
            while (_view.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f)
            {
                yield return new WaitForFixedUpdate();
            }

            _manager.GameModel.EndGame();
            GameCoroutines.DisableCoroutine(_deathAnimationCoroutine);
        }
    }
}