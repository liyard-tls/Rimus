using System.Collections.Generic;
using System.Text;
using Rimus.Scripts.Characters.TargetSelection;
using UnityEngine;

namespace Rimus.Scripts.Characters
{
    public class CharacterEntityDebugUI : MonoBehaviour
    {
        [SerializeField] private bool _showUi = true;
        [SerializeField] private Vector2 _windowSize = new Vector2(320f, 420f);
        [SerializeField] private Vector2 _windowStart = new Vector2(20f, 20f);
        [SerializeField] private Vector2 _windowOffset = new Vector2(340f, 0f);
        [SerializeField] private float _refreshInterval = 0.5f;

        private readonly List<CharacterEntity> _entities = new List<CharacterEntity>();
        private readonly Dictionary<int, Rect> _windowRects = new Dictionary<int, Rect>();
        private float _nextRefreshTime;

        private void Update()
        {
            if (Time.unscaledTime >= _nextRefreshTime)
            {
                RefreshEntities();
                _nextRefreshTime = Time.unscaledTime + _refreshInterval;
            }
        }

        private void OnGUI()
        {
            if (!_showUi)
            {
                return;
            }

            EnsureEntitiesLoaded();

            for (int i = 0; i < _entities.Count; i++)
            {
                CharacterEntity entity = _entities[i];
                if (entity == null)
                {
                    continue;
                }

                int windowId = entity.GetInstanceID();
                Rect windowRect = GetWindowRect(windowId, i);
                _windowRects[windowId] = GUI.Window(windowId, windowRect, id => DrawEntityWindow(id, entity), GetWindowTitle(entity));
            }
        }

        private void DrawEntityWindow(int windowId, CharacterEntity entity)
        {
            GUILayout.BeginVertical();

            DrawIdentity(entity);
            DrawStats(entity);
            DrawHealth(entity);
            DrawSelection(entity);
            DrawSkills(entity);
            DrawQuickActions(entity);

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
        }

        private void DrawIdentity(CharacterEntity entity)
        {
            CharacterDefinition definition = entity.Definition;
            GUILayout.Label($"Definition: {(definition != null ? definition.DisplayName : "none")}");
            GUILayout.Label($"World Pos: {entity.transform.position}");
        }

        private void DrawStats(CharacterEntity entity)
        {
            StatsComponent stats = entity.Stats;
            if (stats == null)
            {
                GUILayout.Label("Stats: missing");
                return;
            }

            CharacterStats currentStats = stats.CurrentStats;
            GUILayout.Label(
                $"Stats HP:{currentStats.MaxHp} MP:{currentStats.MaxMp} ATK:{currentStats.Attack} MATK:{currentStats.MagicAttack} DEF:{currentStats.Defense} MDEF:{currentStats.MagicDefense} SPD:{currentStats.Speed}");
        }

        private void DrawHealth(CharacterEntity entity)
        {
            HealthComponent health = entity.Health;
            if (health == null)
            {
                GUILayout.Label("Health: missing");
                return;
            }

            GUILayout.Label($"Health: {health.CurrentHp}/{health.MaxHp} Dead: {health.IsDead}");
        }

        private void DrawSelection(CharacterEntity entity)
        {
            AttackSelector selector = entity.AttackSelector;
            if (selector == null)
            {
                GUILayout.Label("Selector: missing");
                return;
            }

            GUILayout.Label($"Selector: {(selector.ActiveSelector != null ? selector.ActiveSelector.SelectorType.ToString() : "none")}");
            GUILayout.Label($"Hovered: {FormatTargets(selector.HoveredTargets)}");
            GUILayout.Label($"Selected: {FormatTargets(selector.SelectedTargets)}");

            GUILayout.Space(4f);
            GUILayout.Label("Aim Selector At:");

            for (int i = 0; i < _entities.Count; i++)
            {
                CharacterEntity targetEntity = _entities[i];
                if (targetEntity == null || targetEntity == entity)
                {
                    continue;
                }

                if (GUILayout.Button($"Aim at {GetShortName(targetEntity)}"))
                {
                    selector.UpdateTargeting(targetEntity.transform.position);
                }
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Hover"))
            {
                selector.ClearHover();
            }

            if (GUILayout.Button("Clear Selected"))
            {
                selector.ClearSelection();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSkills(CharacterEntity entity)
        {
            SkillCaster skillCaster = entity.SkillCaster;
            if (skillCaster == null)
            {
                GUILayout.Label("SkillCaster: missing");
                return;
            }

            GUILayout.Space(4f);
            GUILayout.Label($"Current Skill: {(skillCaster.CurrentSkill != null ? skillCaster.CurrentSkill.DisplayName : "none")}");
            GUILayout.Label("Skills:");

            IReadOnlyList<SkillDefinition> knownSkills = skillCaster.KnownSkills;
            if (knownSkills.Count == 0)
            {
                GUILayout.Label("No skills");
            }

            for (int i = 0; i < knownSkills.Count; i++)
            {
                SkillDefinition skill = knownSkills[i];
                if (skill == null)
                {
                    continue;
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button($"Select {skill.DisplayName}"))
                {
                    skillCaster.SelectSkill(skill);
                }

                if (GUILayout.Button($"Cast {skill.DisplayName}"))
                {
                    skillCaster.SelectSkill(skill);
                    skillCaster.TryCastCurrentSkill();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm Targets"))
            {
                entity.AttackSelector?.ConfirmSelection();
            }

            if (GUILayout.Button("Cast Current"))
            {
                skillCaster.TryCastCurrentSkill();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawQuickActions(CharacterEntity entity)
        {
            HealthComponent health = entity.Health;
            if (health == null)
            {
                return;
            }

            GUILayout.Space(4f);
            GUILayout.Label("Quick Actions:");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Damage 1"))
            {
                health.TakeDamage(1);
            }

            if (GUILayout.Button("Damage 5"))
            {
                health.TakeDamage(5);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Heal 1"))
            {
                health.Heal(1);
            }

            if (GUILayout.Button("Heal 5"))
            {
                health.Heal(5);
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Full Restore"))
            {
                health.RestoreToFull();
            }
        }

        private void EnsureEntitiesLoaded()
        {
            if (_entities.Count == 0)
            {
                RefreshEntities();
            }
        }

        private void RefreshEntities()
        {
            _entities.Clear();
            CharacterEntity[] entities = FindObjectsOfType<CharacterEntity>();
            for (int i = 0; i < entities.Length; i++)
            {
                CharacterEntity entity = entities[i];
                if (entity != null)
                {
                    _entities.Add(entity);
                }
            }
        }

        private Rect GetWindowRect(int windowId, int index)
        {
            if (_windowRects.TryGetValue(windowId, out Rect existingRect))
            {
                return existingRect;
            }

            Vector2 position = _windowStart + Vector2.Scale(_windowOffset, new Vector2(index, index > 0 ? Mathf.Floor(index / 3f) : 0f));
            Rect newRect = new Rect(position.x, position.y, _windowSize.x, _windowSize.y);
            _windowRects[windowId] = newRect;
            return newRect;
        }

        private static string GetWindowTitle(CharacterEntity entity)
        {
            return $"Entity Debug: {GetShortName(entity)}";
        }

        private static string GetShortName(CharacterEntity entity)
        {
            if (entity == null)
            {
                return "null";
            }

            return entity.Definition != null ? entity.Definition.DisplayName : entity.name;
        }

        private static string FormatTargets(IReadOnlyList<Targetable> targets)
        {
            if (targets == null || targets.Count == 0)
            {
                return "none";
            }

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < targets.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                Targetable target = targets[i];
                builder.Append(target != null ? target.name : "null");
            }

            return builder.ToString();
        }
    }
}
