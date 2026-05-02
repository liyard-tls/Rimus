using System.Collections;

namespace Rimus.Scripts.Battle.Interfaces
{
    public interface IBattleCommand
    {
        IEnumerator Execute(BattleContext context);
    }
}