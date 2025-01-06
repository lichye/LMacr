//AE APIs
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.JobApi;

//Dalamud APIs
using Dalamud.Game.ClientState.Objects.Types;

//System APIs
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using LM.Reaper.Setting;
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
        UpdateShroudGauge();
        UpdateTargetWithoutDealthDesign();
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

    private void UpdateShroudGauge()
    {
        if(Core.Resolve<JobApi_Reaper>().ShroudGauge >=50)
            ReaperBattleData.Instance.IsAbleDoubleEnshroud = true;
    }

    private void UpdateTargetWithoutDealthDesign()
    {
        // TargetMgr.Instance.Enemys.Values
        // Dictionary<uint, IGameObject> dict = new Dictionary<uint, IGameObject>();
        // Core.Resolve<MemApiTarget>().GetNearbyGameObjects(5, dict);
        // int count = 0;
        // foreach (KeyValuePair<uint, IGameObject> v in dict)
        // {
        //     if (!v.Value.HasMyAura(AurasDefine.DeathsDesign))
        //         count++;
        // }
        // ReaperBattleData.Instance.targetWithoutDeathsDesign = count;

    }
}