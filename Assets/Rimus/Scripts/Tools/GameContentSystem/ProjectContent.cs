using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rimus.Scripts.Tools.GameContentSystem
{
    public class ProjectContent : SceneContent
    {
        private static ProjectContent _instance;

        public void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("Multiple instances of ProjectContent found! Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}