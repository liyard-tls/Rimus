using UnityEngine;

namespace Rimus.Scripts.Map
{
    [CreateAssetMenu(fileName = "MapNodeAssets", menuName = "MapNodeAssets", order = 0)]
    public class MapNodeAssets : ScriptableObject
    {
        public Sprite FilledBackground;
        public Sprite DraftedBackground;
        public Sprite Lid;
        public Sprite MonsterIcon;
        public Sprite HumanIcon;
        public Sprite BossIcon;
        public Sprite HealIcon;
        public Sprite TreasureIcon;
        public Sprite ExclamationIcon;
        public Sprite QuestionIcon;
        
        public Color DefaultColor;
        public Color CurrentColor;
        public Color PassedColor;
        public Color FailedColor;
        public Color MonsterColor;
        public Color HumanColor;
        public Color BossColor;
        public Color HealColor;
        public Color TreasureColor;
        public Color ExclamationColor;
        public Color QuestionColor;
        
    }
}