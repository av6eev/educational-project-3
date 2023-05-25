using Player;
using UnityEditor;
using UnityEngine;

namespace FOV
{
    [CustomEditor(typeof(PlayerView))]
    public class FOVEditor : Editor
    {
        private void OnSceneGUI()
        {
            var fov = (PlayerView) target;
            var position = fov.transform.position;

            Handles.color = Color.white;
            Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360, fov.ViewRadius);

            var viewAngle1 = fov.DirectionFromAngle(-fov.ViewAngle / 2, false);
            var viewAngle2 = fov.DirectionFromAngle(fov.ViewAngle / 2, false);
            
            Handles.DrawLine(position, position + viewAngle1 * fov.ViewRadius);
            Handles.DrawLine(position, position + viewAngle2 * fov.ViewRadius);
            Handles.color = Color.red;
            
            foreach (var visibleTarget in fov.VisibleTargets)
            {
                Handles.DrawLine(position, visibleTarget.position);
            }
        } 
    }
}