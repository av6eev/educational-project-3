using System.Threading.Tasks;
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
            _model.OnDealDamage -= UpdateHealthBar;
        }

        public void Activate()
        {
            _view.Disable();
            
            _model.OnPlayerCreated += InitPlayer;
            _model.OnPlayerRemoved += DestroyPlayer;
            _model.OnPlayerMove += MovePlayer;
            _model.OnPlayerAttack += AttackEnemy;
            _model.OnDealDamage += UpdateHealthBar;
        }

        private void UpdateHealthBar()
        {
            _view.UpdateHealthBar(_model.MaxHealth, _model.CurrentHealth);
        }

        private async void AttackEnemy()
        {
            if (_view.VisibleTargets.Count == 0) return;
            if (!_model.Id.Equals(_manager.GameModel.ActivePlayer.Id)) return;
            
            var enemy = _manager.GameModel.GetEnemyModel();
            var damage = _model.AttackDamage * ((100 - enemy.Resistance) / 100);
            
            _view.PlayAttackAnimation();

            enemy.DealDamage(damage);
            
            await Task.Delay(1000);
            _manager.GameModel.ChangeTurn();
        }

        private async void MovePlayer(Vector3 direction, Vector3 newAngle)
        {
            var cells = _manager.FloorModel.Cells;
            var position = _model.Position + direction;
            
            cells.TryGetValue(new Vector3(position.x, 0, position.z), out var newCell);

            if (newCell is not { IsPlayable: true })
            {
                _manager.GameModel.ChangeTurn();
                return;
            }
            
            cells[new Vector3(_manager.GameModel.ActivePlayer.Position.x, 0, _manager.GameModel.ActivePlayer.Position.z)].IsActive = false;
            newCell.IsActive = true;

            _model.Direction = direction;
            _model.Position += direction;
            _model.Angle = newAngle;

            _view.PlayWalkAnimation(true);

            await Task.Delay(3000);            
            
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
    }
}