//AE APIs
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.JobApi;
using AEAssist.CombatRoutine.Module.Target;

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
            case SpellsDefine.ArcaneCircle:
                ReaperBattleData.Instance.HoldPlentifulHarvest = true;
                break;
            case SpellsDefine.PlentifulHarvest:
                ReaperBattleData.Instance.HoldPlentifulHarvest = false;
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
        UpdateTargetNearbyCount();
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
        if(Core.Resolve<JobApi_Reaper>().ShroudGauge >=50 
        && SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds > 5000)
            ReaperBattleData.Instance.IsAbleDoubleEnshroud = true;
    }

    private void UpdateTargetWithoutDealthDesign()
    {
        Dictionary<uint, IBattleChara> list = TargetMgr.Instance.EnemysIn25;

        // Dictionary<uint, IGameObject> dict = new Dictionary<uint, IGameObject>();
        // Core.Resolve<MemApiTarget>().GetNearbyGameObjects(5, dict);
        int count = 0;
        foreach (KeyValuePair<uint, IBattleChara> v in list)
        {   
            // if the target doesn't have DeathsDesign aura
            if (!v.Value.HasMyAuraWithTimeleft(AurasDefine.DeathsDesign, ReaperSettings.Instance.ShadowofDeath_time) &&
                v.Value.Distance(Core.Me)<SettingMgr.GetSetting<GeneralSettings>().AttackRange)
                count++;
        }
        ReaperBattleData.Instance.targetWithoutDeathsDesign = count;

    }

    private void UpdateTargetNearbyCount()
    {
        ReaperBattleData.Instance.targetNearbyCount = TargetHelper.GetNearbyEnemyCount(Core.Me, 5, 5);
    }
}