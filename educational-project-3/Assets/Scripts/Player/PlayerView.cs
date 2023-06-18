using System.Collections;
using System.Collections.Generic;
using Arrow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerView : MonoBehaviour
    {
        public Animator Animator;

        public Canvas InfoRoot;
        public Image HealthBar;
        public TextMeshPro Text;
        public GameObject Root;
        
        public Transform ArrowSpawnPoint;
        public ArrowView ArrowPrefab;
        
        public LayerMask TargetMask;
        public LayerMask EnvironmentMask;
        
        [Range(0, 360)] public float ViewAngle;
        public float ViewRadius;
        
        [HideInInspector] public List<Transform> VisibleTargets = new();

        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        private static readonly int IsDeath = Animator.StringToHash("IsDeath");

        private void Start()
        {
            StartCoroutine(FindTargetsWithDelay(.2f));
        }

        public void DestroyPlayer()
        {
            Destroy(Root);
        }

        public void Disable()
        {
            Root.SetActive(false);
        }
        
        public void Enable()
        {
            Root.SetActive(true);
        }

        public void PlayWalkAnimation(bool state)
        {
            Animator.SetBool(IsWalking, state);
        }
        
        public void PlayAttackAnimation(PlayerClassType type, Transform target)
        {
            Animator.SetTrigger(IsAttacking);

            if (type == PlayerClassType.Archer)
            {
                var arrow = Instantiate(ArrowPrefab, ArrowSpawnPoint.position, Quaternion.Euler(90f, 0, 0));

                if (arrow != null)
                {
                    arrow.OnInstantiate(target);
                }
            }
        }

        public void UpdateHealthBar(float maxHealth, float currentHealth)
        {
            HealthBar.fillAmount = currentHealth / maxHealth;
        }
        
        public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        private IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleTargets();
            }
        }

        private void FindVisibleTargets()
        {
            VisibleTargets.Clear();
            
            var targets = Physics.OverlapSphere(transform.position, ViewRadius, TargetMask);

            foreach (var element in targets)
            {
                var target = element.transform;
                var directionToTarget= (target.position - transform.position).normalized;

                if (!(Vector3.Angle(transform.forward, directionToTarget) < ViewAngle / 2)) continue;
                
                var distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, EnvironmentMask))
                {
                    VisibleTargets.Add(target);
                }
            }
        }

        public void PlayDeathAnimation()
        {
            Animator.SetTrigger(IsDeath);
        }
    }
}