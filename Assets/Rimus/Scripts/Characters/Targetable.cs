using UnityEngine;
using Rimus.Scripts.Tools;

namespace Rimus.Scripts.Characters
{
    public class Targetable : MonoBehaviour
    {
        public void SetOnTargeted(bool isTargeted)
        {
            Log.Info($"Targetable '{name}' SetOnTargeted({isTargeted})");
            transform.localScale = isTargeted ? Vector3.one * 1.2f : Vector3.one;
        }
    }
}
