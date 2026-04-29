using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rimus.Scripts.Tools.GameContentSystem
{
    
    [CreateAssetMenu(menuName = "Game/Content Database")]
    public class ContentDatabase : ScriptableObject
    {
        [SerializeField] private List<IGameContent> content = new();

        public IReadOnlyList<IGameContent> Content => content;

#if UNITY_EDITOR
        public void Editor_SetItems(List<IGameContent> value)
        {
            content = value;
        }
#endif
    }
}