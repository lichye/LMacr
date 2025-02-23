using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.JobApi;
using static System.Windows.Forms.Design.AxImporter;
using System.CodeDom;
using AEAssist.Avoid;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.Extension;
using AEAssist.Module.Avoid;
using LM.Reaper.Setting;
namespace LM.Reaper.SlotResolvers.GCD;
using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.Types;

public class GCD_ShadowofDeath : ISlotResolver
{
    //In this slot resolver，we will use one of the following skills:
    // WhorlOfDeath
    // ShadowOfDeath Lv.35

    private Spell GetSpell()
    {   
        
        //if in aoe mode
        if (ReaperRotationEntry.QT.GetQt(QTKey.AOE) && ReaperBattleData.Instance.targetWithoutDeathsDesign>=2)
        {
            return SpellsDefine.WhorlOfDeath.GetSpell();
        }

        if(ReaperBattleData.Instance.AutoDoubleEnshroud){
            //if the target will not have the DeathsDesign debuff in the next X seconds, we will use the ShadowOfDeath skill
            if (!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.DeathsDesign, ReaperSettings.Instance.ShadowofDeath_time))
                return SpellsDefine.ShadowOfDeath.GetSpell();

            if(ReaperBattleData.Instance.DoOneReaping){
                ReaperBattleData.Instance.DoOneReaping = false;
                return SpellsDefine.VoidReaping.GetSpell();
            }
                
            //The second usage of ShadowOfDeath,turn off the automatic use of ShadowOfDeath
            ReaperBattleData.Instance.AutoDoubleEnshroud = false;
            return SpellsDefine.ShadowOfDeath.GetSpell();
        }

        return SpellsDefine.ShadowOfDeath.GetSpell();
    }

    public int Check()
    {   
        //Level Check
        if (Core.Me.Level < 10)
            return -1;

        //Target touchable check  
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) > SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -2;
            
        //Buff confiction Check
        if (Core.Me.HasAura(AurasDefine.SoulReaver) || 
            Core.Me.HasAura(AurasDefine.Executioner))
            return -3;

        //AutoDoubleEnshroud Check
        if(ReaperBattleData.Instance.AutoDoubleEnshroud){
            return 1;
        }

        // In Double Enshroud Trigger
        if (ReaperSettings.Instance.DoubleEnshroud&&Core.Me.Level>=90){
            if(SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds < 10000){
                return -101;
            }       
        }

        if (!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.DeathsDesign, ReaperSettings.Instance.ShadowofDeath_time))
            return 1;

         //if in aoe mode
        if (ReaperRotationEntry.QT.GetQt(QTKey.AOE) && ReaperBattleData.Instance.targetWithoutDeathsDesign>=2)
        {
            return 2;
        }

        // if glutony is ready and shadowofdeath will lose effect

        if (!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.DeathsDesign, ReaperSettings.Instance.GCD_Time*2+ReaperSettings.Instance.AnimationLock*3))
        {
            if (SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMicroseconds<ReaperSettings.Instance.GCD_Time*2+ReaperSettings.Instance.AnimationLock*3)
            {
                return 3;
            }
        }

        //Normally not use
        return -100;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }

}