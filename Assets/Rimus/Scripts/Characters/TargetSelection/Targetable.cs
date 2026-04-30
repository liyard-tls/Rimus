using Rimus.Scripts.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rimus.Scripts.Characters.TargetSelection
{
    public class Targetable : MonoBehaviour
    {
        public UnityEvent<bool> OnTargeted = new UnityEvent<bool>();
        
        public void SetOnTargeted(bool isTargeted)
        {
            Log.Info($"Targetable '{name}' SetOnTargeted({isTargeted})");
            //transform.localScale = isTargeted ? Vector3.one * 1.2f : Vector3.one;
            OnTargeted.Invoke(isTargeted);
        }
    }
}
