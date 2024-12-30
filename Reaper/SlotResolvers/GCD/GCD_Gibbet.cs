using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using AEAssist.JobApi;
using LM.Reaper.Setting;

namespace LM.Reaper.SlotResolvers.GCD;

public class GCD_Gibbet : ISlotResolver
{
    //In this slot resolver，we will use one of the following skills:
    // Guillotine Lv.70
    // Gibbet Lv.70 attack from the side
    // Gallows Lv.70 attack from the back

    private Spell GetSpell()
    {
        if (ReaperRotationEntry.QT.GetQt(QTKey.AOE))
        {
            var aoeCount = TargetHelper.GetNearbyEnemyCount(Core.Me, 8, 8);
            if (aoeCount >= 3)
                return SpellsDefine.Guillotine.GetSpell();
        }

        // Positional
        if(ReaperSettings.Instance.careAboutPos &&
        Core.Me.GetCurrTarget().HasPositional()){
            // Two side attacks
            if(Core.Me.HasAura(AurasDefine.EnhancedGallows) && 
                Core.Me.HasAura(AurasDefine.Executioner) &&
                Core.Me.Level >= 96 &&
                Core.Resolve<MemApiTarget>().IsBehind)
                return SpellsDefine.ExGallows.GetSpell();//Back

            if (Core.Me.HasAura(AurasDefine.EnhancedGallows)&&
                Core.Me.HasAura(AurasDefine.SoulReaver) &&
                Core.Resolve<MemApiTarget>().IsBehind)
                return SpellsDefine.Gallows.GetSpell();//Back

            if (Core.Me.HasAura(AurasDefine.EnhancedGibbet) && 
                Core.Me.HasAura(AurasDefine.Executioner) &&
                Core.Me.Level >= 96 &&
                Core.Resolve<MemApiTarget>().IsFlanking)
                return SpellsDefine.ExGibbet.GetSpell();//Back

            if (Core.Me.HasAura(AurasDefine.EnhancedGibbet) && 
                Core.Me.HasAura(AurasDefine.SoulReaver) &&
                Core.Resolve<MemApiTarget>().IsFlanking)
                return SpellsDefine.Gibbet.GetSpell();//Back
        }
        // Without Positional
        else{

            if(Core.Me.HasAura(AurasDefine.EnhancedGallows) && 
                Core.Me.HasAura(AurasDefine.Executioner) &&
                Core.Me.Level >= 96)
                return SpellsDefine.ExGallows.GetSpell();//Back

            if (Core.Me.HasAura(AurasDefine.EnhancedGallows)&&
                Core.Me.HasAura(AurasDefine.SoulReaver))
                return SpellsDefine.Gallows.GetSpell();//Back

            if (Core.Me.HasAura(AurasDefine.EnhancedGibbet) && 
                Core.Me.HasAura(AurasDefine.Executioner) &&
                Core.Me.Level >= 96)
                return SpellsDefine.ExGibbet.GetSpell();//Back

            if (Core.Me.HasAura(AurasDefine.EnhancedGibbet) && 
                Core.Me.HasAura(AurasDefine.SoulReaver))
                return SpellsDefine.Gibbet.GetSpell();//Back

        }
        
        
        //Open
        if(ReaperSettings.Instance.BaseGCD_BehindFirst)
            return SpellsDefine.Gallows.GetSpell();
        else
            return SpellsDefine.Gibbet.GetSpell();
        
    }

    public int Check()
    {   
        //Level Check
        if (Core.Me.Level < 70)
            return -1;
        
        //Buff Check
        if (!Core.Me.HasAura(AurasDefine.SoulReaver) && !Core.Me.HasAura(AurasDefine.Executioner))
            return -2;

        //Target touchable check
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) >SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -3;
        
        //Positional Check
        if(ReaperSettings.Instance.careAboutPos && Core.Me.GetCurrTarget().HasPositional()){
            //if we have the EnhancedGallows and we are not behind the target, we will not use this solver
            if (Core.Me.HasAura(AurasDefine.EnhancedGallows) && !Core.Resolve<MemApiTarget>().IsBehind)
                return -4;
            
            //if we have the EnhancedGibbet and we are not flanking the target, we will not use this solver
            if (Core.Me.HasAura(AurasDefine.EnhancedGibbet) && !Core.Resolve<MemApiTarget>().IsFlanking)
                return -5;
        }
        
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}