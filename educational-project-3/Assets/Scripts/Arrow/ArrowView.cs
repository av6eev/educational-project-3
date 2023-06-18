using UnityEngine;

namespace Arrow
{
    public class ArrowView : MonoBehaviour
    {
        private Transform _target;
        public float Speed = .3f;

        public void OnInstantiate(Transform target)
        {
            _target = target;
        }

        public void Update()
        {
            if (_target == null)
            {
                Destroy(gameObject);
                return;
            }

            var targetPosition = _target.position;
            var direction = new Vector3(targetPosition.x, 1f, targetPosition.z) - transform.position;
            var distanceAtFrame = Speed * Time.deltaTime;

            if (direction.magnitude <= distanceAtFrame)
            {
                Destroy(gameObject);
                return;
            }

            transform.Translate(direction.normalized * distanceAtFrame, Space.World);
        }
    }
}