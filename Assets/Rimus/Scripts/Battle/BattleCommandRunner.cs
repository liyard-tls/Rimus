using System.Collections;
using System.Collections.Generic;
using Rimus.Scripts.Battle.Interfaces;
using UnityEngine;

namespace Rimus.Scripts.Battle
{
    public class BattleCommandRunner : MonoBehaviour
    {
        private readonly Queue<IBattleCommand> _commands = new Queue<IBattleCommand>();
        private BattleContext _context = new BattleContext();

        public void Enqueue(IBattleCommand command)
        {
            _commands.Enqueue(command);
        }

        public IEnumerator ExecuteAll()
        {
            while (_commands.Count > 0)
            {
                var command = _commands.Dequeue();
                yield return StartCoroutine(command.Execute(_context));
                
            }
        }
    }
}