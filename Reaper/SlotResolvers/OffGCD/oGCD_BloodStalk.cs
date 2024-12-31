using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using static System.Windows.Forms.Design.AxImporter;
using LM.Reaper.Setting;

namespace LM.Reaper.SlotResolvers.OffGCD;

public class oGCD_BloodStalk : ISlotResolver
{
    //In this slot resolver，we will use one of the following skills:
    // BloodStalk Lv.50
    // Gallows Lv.70 attack the back
    // Gibbet Lv.70 attack the Frank
    private Spell GetSpell(){

        // Gibbet Back
        if (Core.Me.HasAura(AurasDefine.EnhancedGallows) && Core.Resolve<MemApiTarget>().IsBehind)
        {
            return SpellsDefine.UnveiledGallows.GetSpell();
        }

        // Gibbet Flank
        if (Core.Me.HasAura(AurasDefine.EnhancedGibbet) && Core.Resolve<MemApiTarget>().IsFlanking)
        {
            return SpellsDefine.UnveiledGibbet.GetSpell();    
        }
        
        return SpellsDefine.BloodStalk.GetSpell();
    }   

    public int Check()
    {
        // Level Check
        if (Core.Me.Level < 50)
            return -1;

        // GCD confiction Check
        if (GCDHelper.GetGCDCooldown() < ReaperSettings.Instance.AnimationLock)
            return -2;
        
        // Skill Ready Check
        if (Core.Resolve<JobApi_Reaper>().SoulGauge < 50 || !SpellsDefine.BloodStalk.IsReady())
            return -2;
        
        // ShroudGauge Overflow Check
        if (Core.Resolve<JobApi_Reaper>().ShroudGauge == 100)
            return -3;
        
        //Buff confiction Check
        if (Core.Me.HasAura(AurasDefine.SoulReaver) || 
            Core.Me.HasAura(AurasDefine.Executioner) ||
            Core.Me.HasAura(AurasDefine.Enshrouded) ||
            Core.Me.HasAura(AurasDefine.ImmortalSacrifice) ||
            Core.Me.HasAura(AurasDefine.PerfectioParata)
            )
            return -5;
        
        //DeathsDesign Check
        if (!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.DeathsDesign,
            ReaperSettings.Instance.GCD_Time+ReaperSettings.Instance.AnimationLock*3))
            return -6;
        

        //Combo Check
        if (Core.Resolve<MemApiSpell>().GetComboTimeLeft().TotalMilliseconds < 2500)
            if (Core.Resolve<MemApiSpell>().GetLastComboSpellId() == SpellsDefine.Slice ||
                Core.Resolve<MemApiSpell>().GetLastComboSpellId() == SpellsDefine.WaxingSlice)
                return -8;

        //Position check
        if(ReaperSettings.Instance.careAboutPos && Core.Me.GetCurrTarget().HasPositional()){
            //if we are on the back of the Target and we have the aura of EnhancedGibbet, we will not use this solver
            if (Core.Me.HasAura(AurasDefine.EnhancedGibbet) && !Core.Resolve<MemApiTarget>().IsFlanking)
            {
                return -11;
            }

            //if we are on the side of the Target and we have the aura of EnhancedGallows, we will not use this solver
            if (Core.Me.HasAura(AurasDefine.EnhancedGallows) && !Core.Resolve<MemApiTarget>().IsBehind)
            {
                return -12;
            }
        }
        
                
        //Gluttony check
        if (SpellsDefine.Gluttony.CoolDownInGCDs(5) && Core.Resolve<JobApi_Reaper>().SoulGauge < 100)
            return -13;
        
        
        // if( SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds<10000 &&
        //     Core.Resolve<JobApi_Reaper>().ShroudGauge < 50)
        //     return 1;

        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}