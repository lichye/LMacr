//AE APIs
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.JobApi;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.CombatRoutine.View;

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
            case SpellsDefine.ExGallows:
                MeleePosHelper.Clear();
                break;
            case SpellsDefine.ExGibbet:
                MeleePosHelper.Clear();
                break;
            case SpellsDefine.Gallows:
                MeleePosHelper.Clear();
                break;
            case SpellsDefine.Gibbet:
                MeleePosHelper.Clear();
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
        UpdateMeleePos();
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

    private void UpdateMeleePos(){
        var t2 = GCDHelper.GetGCDCooldown();
        
        if (!ReaperSettings.Instance.DrawPosition){
            MeleePosHelper.Clear();
            return;
        }

        if (Core.Me.HasAura(AurasDefine.Enshrouded)){
            MeleePosHelper.Clear();
            return;
        }

        if  (Core.Me.HasAura(AurasDefine.TrueNorth)){
            MeleePosHelper.Clear();
            return;
        }
        
        if (Core.Me.HasAura(AurasDefine.EnhancedGibbet)){
            if(Core.Resolve<JobApi_Reaper>().SoulGauge >=50)
                MeleePosHelper.Draw(MeleePosHelper.Pos.Flank, 100);
            else
                MeleePosHelper.Clear();
            if(Core.Me.HasAura(AurasDefine.SoulReaver)||Core.Me.HasAura(AurasDefine.Executioner)){
                MeleePosHelper.Draw(MeleePosHelper.Pos.Flank, (int)t2 / 25+10);
            }
                
            return;
        }
        
        if (Core.Me.HasAura(AurasDefine.EnhancedGallows)){
            if(Core.Resolve<JobApi_Reaper>().SoulGauge >=50)
                MeleePosHelper.Draw(MeleePosHelper.Pos.Behind, 100);
            else
                MeleePosHelper.Clear();
            if(Core.Me.HasAura(AurasDefine.SoulReaver)||Core.Me.HasAura(AurasDefine.Executioner))
                MeleePosHelper.Draw(MeleePosHelper.Pos.Behind, (int)t2 / 25+10);
            return;
        }
        
        

        if (ReaperSettings.Instance.BaseGCD_BehindFirst){
            MeleePosHelper.Draw(MeleePosHelper.Pos.Behind, 100);
        }else{
            MeleePosHelper.Draw(MeleePosHelper.Pos.Flank, 100);
        }

    }
}