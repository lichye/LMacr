using System.Threading.Tasks;
using LM.Reaper.Setting;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using System.Xml.Linq;

namespace LM.Reaper;

/// <summary>
/// 事件回调处理类 参考接口里的方法注释
/// </summary>
public class ReaperRotationEventHandler : IRotationEventHandler
{
    public static long times;
    private bool 易伤;
    public async Task OnPreCombat()
    {
    }

    public void OnResetBattle()
    {
        // 重置战斗中缓存的数据
        ReaperBattleData.Instance = new();
        times = 0;
        易伤 = false;
    }

    public async Task OnNoTarget()
    {
        await Task.CompletedTask;
    }

    public void OnSpellCastSuccess(Slot slot, Spell spell)
    {

    }

    public void AfterSpell(Slot slot, Spell spell)
    {
        switch (spell.Id)
        {
            case SpellsDefine.VoidReaping:
                AI.Instance.BattleData.CurrGcdAbilityCount = 1;
                break;
            case SpellsDefine.CrossReaping:
                AI.Instance.BattleData.CurrGcdAbilityCount = 1;
                break;
            default:
                AI.Instance.BattleData.CurrGcdAbilityCount = 2;
                break;
        }
    }

    public void OnBattleUpdate(int currTimeInMs)
    {
        if (!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.DeathsDesign, 100))
        {
            if (易伤)
            {
                times = AI.Instance.BattleData.CurrBattleTimeInMs / 1000;
                易伤 = false;
            }
        }
        else 易伤 = true;




    }

    public void OnEnterRotation()
    {

    }

    public void OnExitRotation()
    {

    }

    public void OnTerritoryChanged()
    {

    }
}