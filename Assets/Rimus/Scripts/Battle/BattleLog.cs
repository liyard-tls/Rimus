using System.Collections.Generic;
using Rimus.Scripts.Battle.Interfaces;
using Rimus.Scripts.Tools;

namespace Rimus.Scripts.Battle
{
    public class BattleLog
    {
        private readonly List<string> _logEntries;

        public BattleLog()
        {
            _logEntries = new List<string>();
        }

        public void AddEntry(IBattleCommand command, string message)
        {
            string entry = $"[{command.GetType().Name}]: {message}";
            _logEntries.Add(entry);
            Log.BattleLog(entry);
        }
        
        public IEnumerable<string> GetEntries()
        {
            return _logEntries;
        }

        public void Clear()
        {
            _logEntries.Clear();
        }
        
        public void PrintLogs()
        {
            foreach (var entry in _logEntries)
            {
                Log.BattleLog(entry);
            }
        }
    }
}