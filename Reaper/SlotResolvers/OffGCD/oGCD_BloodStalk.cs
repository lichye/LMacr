using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using static System.Windows.Forms.Design.AxImporter;

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
        //if we donot have the cooldown of the GCD, we will not use this solver
        if (GCDHelper.GetGCDCooldown() < 600)
            return -1;
        
        //if we donot have enough SoulGauge, we will not use this solver
        if (Core.Resolve<JobApi_Reaper>().SoulGauge < 50)
            return -2;
        
        //if we have 100 ShroudGauge, we will not use this solver
        if (Core.Resolve<JobApi_Reaper>().ShroudGauge == 100)
            return -3;
        
        //if we have the aura of SoulReaver or Executioner, we will not use this solver
        if (Core.Me.HasAura(AurasDefine.SoulReaver) || Core.Me.HasAura(AurasDefine.Executioner))
            return -5;
        
        //if we the target will not have the aura of DeathsDesign, we will not use this solver
        if (!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.DeathsDesign, 2500))
            return -6;
        
        //if we don't have the aura of Enshrouded, we will not use this solver
        if (!SpellsDefine.BloodStalk.IsReady())
            return -7;

        //if we will loss the combo, we will not use this solver
        if (Core.Resolve<MemApiSpell>().GetComboTimeLeft().TotalMilliseconds < 2500)
            if (Core.Resolve<MemApiSpell>().GetLastComboSpellId() == SpellsDefine.Slice ||
                Core.Resolve<MemApiSpell>().GetLastComboSpellId() == SpellsDefine.WaxingSlice)
                return -8;

        //if we are in the burst mode, we will not use this solver
        if (Core.Me.HasAura(AurasDefine.Enshrouded))
            return -9;

        //if we recently used the spell of PlentifulHarvest, we will not use this solver
        if (SpellsDefine.PlentifulHarvest.RecentlyUsed(1000))
            return -10;


        //Below are the conditions that we can optimize the use of the skills
        
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

        if( SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds<10000 &&
            Core.Resolve<JobApi_Reaper>().ShroudGauge < 50)
            return 1;
        
        //if Gluttony is coming and we have enough ShroudGauge, we will not use this solver
        if (SpellsDefine.Gluttony.CoolDownInGCDs(10) && 
            Core.Resolve<JobApi_Reaper>().SoulGauge < 100)
            return -13;
        
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}