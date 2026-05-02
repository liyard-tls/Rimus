using System;
using UnityEngine;

namespace Rimus.Scripts.Characters
{
    public class PlayerControlPanel : MonoBehaviour
    {
        private void DrawWindow()
        {
            GUILayout.Label("Player Control Panel");
            if (GUILayout.Button("Test Command"))
            {
                Debug.Log("Test Command Executed");
            }
        }
        
        public void OnGUI()
        {
            DrawWindow();
        }
    }
}