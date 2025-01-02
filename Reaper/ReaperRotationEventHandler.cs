using System.Threading.Tasks;
using LM.Reaper.Setting;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using System.Xml.Linq;
using AEAssist.JobApi;

namespace LM.Reaper;

/// <summary>
/// 事件回调处理类 参考接口里的方法注释
/// </summary>
public class ReaperRotationEventHandler : IRotationEventHandler
{
    public static long times;
    public async Task OnPreCombat()
    {
    }

    public void OnResetBattle()
    {
        // 重置战斗中缓存的数据
        ReaperBattleData.Instance = new();
        times = 0;
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
        if(Core.Resolve<JobApi_Reaper>().ShroudGauge >=50)
            ReaperBattleData.Instance.IsAbleDoubleEnshroud = true;

        // if (SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds <12000 &&
        //     SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds > 8000 &&
        //     !ReaperBattleData.Instance.IsAbleDoubleEnshroud){
        //         if(Core.Resolve<JobApi_Reaper>().ShroudGauge >50)
        //             ReaperBattleData.Instance.IsAbleDoubleEnshroud = true;
        //     }
        
        // if (SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds >90000)
        //     ReaperBattleData.Instance.IsAbleDoubleEnshroud = false;
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