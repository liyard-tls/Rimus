namespace Rimus.Scripts.Battle
{
    public class BattleContext
    {
        public BattleCalculator BattleCalculator { get; }
        public BattleLog BattleLog { get; }

        public BattleContext()
        {
            BattleCalculator = new BattleCalculator();
            BattleLog = new BattleLog();
        }
        
        public BattleContext(BattleCalculator battleCalculator, BattleLog battleLog)
        {
            BattleCalculator = battleCalculator;
            BattleLog = battleLog;
        }
        
        
    }
}