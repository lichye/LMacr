using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using static System.Windows.Forms.Design.AxImporter;
using System.CodeDom;

namespace LM.Reaper.SlotResolvers.GCD;

public class GCD_ShadowofDeath : ISlotResolver
{
    //In this slot resolver，we will use one of the following skills:
    // WhorlOfDeath
    // ShadowOfDeath Lv.35

    private Spell GetSpell()
    {   
        //if in aoe mode
        if (ReaperRotationEntry.QT.GetQt(QTKey.AOE))
        {
            var aoeCount = TargetHelper.GetNearbyEnemyCount(Core.Me, 5, 5);
            
            //if there are more than 2 enemies around us and we are at least level 35
            if (aoeCount >= 2&& Core.Me.Level >= 35)
                return SpellsDefine.WhorlOfDeath.GetSpell();
        }

        return SpellsDefine.ShadowOfDeath.GetSpell();
    }

    public int Check()
    {   
        //if we are not level 10, we will not use this solver
        if (Core.Me.Level < 10)
            return -20;

        //if we cannot touch the target, we will not use this solver    
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) > SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -10;
            
        //if we has the SoulReaver or Executioner buff, we will not use this solver
        if (Core.Me.HasAura(AurasDefine.SoulReaver) || Core.Me.HasAura(AurasDefine.Executioner))
            return -21;
        
        //if we has used BloodStalk or Gluttony in the last 1.5 seconds, we will not use this solver
        if (SpellsDefine.BloodStalk.RecentlyUsed(1500) || SpellsDefine.Gluttony.RecentlyUsed(1500))
            return -22;

        
        // if (ReaperRotationEntry.QT.GetQt(QTKey.ShadowofDeath) == false)
        //     return -3;


        //if the ArcaneCircle skill is in cooldown, we will use the ShadowOfDeath skill
        if (SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds >= 10000)
        {   
            //if the target does not have the DeathsDesign debuff, we will use the ShadowOfDeath skill
            if (!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.DeathsDesign, GCDHelper.GetGCDDuration() + GCDHelper.GetGCDCooldown() + 200))
                return 1;
        }

        //if the target will not have the DeathsDesign debuff in the next 5 seconds, we will use the ShadowOfDeath skill
        if (!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.DeathsDesign, 5000))
            return 2;

        // if (Core.Resolve<MemApiBuff>().GetAuraTimeleft(Core.Me.GetCurrTarget(), AurasDefine.DeathsDesign, true) - SpellsDefine.Gluttony.GetSpell().Cooldown.TotalMilliseconds > 0 &&
        //    Core.Resolve<MemApiBuff>().GetAuraTimeleft(Core.Me.GetCurrTarget(), AurasDefine.DeathsDesign, true) - SpellsDefine.Gluttony.GetSpell().Cooldown.TotalMilliseconds < 5000 &&
        //    SpellsDefine.Gluttony.GetSpell().Cooldown.TotalMilliseconds < 30000)
        //     return 3;

        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}