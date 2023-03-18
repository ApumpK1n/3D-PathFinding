using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pumpkin.AI.BehaviorTree
{
    [CustomEditor(typeof(RobotMovementController))]
    public class RobotMovementControllerEidtor : Editor
    {
        private RobotMovementController m_Tree;
        private void OnEnable()
        {
            m_Tree = target as RobotMovementController;

            EditorApplication.update += RedrawView;
        }


        void RedrawView()
        {
            Repaint();

        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Tick"))
            {
                if (m_Tree != null)
                {
                    m_Tree.GetPath();
                }
            }
        }
    }
}
