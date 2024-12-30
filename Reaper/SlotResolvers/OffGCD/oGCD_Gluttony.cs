using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using static System.Windows.Forms.Design.AxImporter;
using AEAssist.JobApi;
using LM.Reaper.Setting;

namespace LM.Reaper.SlotResolvers.OffGCD;

public class offGCD_Gluttony : ISlotResolver
{
    //In this slot resolver，we will use one of the following skills:
    // Gluttony Lv.76
    private Spell GetSpell()
    {
        return SpellsDefine.Gluttony.GetSpell();
    }
    public int Check()
    {    
        //if we are not level 76, we will not use this solver
        if (Core.Me.Level < 76)
            return -1;
        
        //if we don't have the SoulGauge, we will not use this solver
        if (Core.Resolve<JobApi_Reaper>().SoulGauge < 50)
            return -2;

        //if we can't use GCD, we will not use this solver
        if (GCDHelper.GetGCDCooldown() < 600)
            return -3;
        
        //if the Gluttony skill is not ready, we will not use this solver
        if (!SpellsDefine.Gluttony.IsReady())
            return -4;
        
        //if the target does not have the DeathsDesign debuff, we will not use this solver
        if (!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.DeathsDesign, 7500))
            return -6;
        
        //if we have the Enshrouded buff, we will not use this solver
        if (Core.Me.HasAura(AurasDefine.Enshrouded)||Core.Me.HasAura(AurasDefine.SoulReaver))
            return -7;
        
        //if we have ImortalSacrifice buff, we will not use this solver
        if (Core.Me.HasAura(AurasDefine.ImmortalSacrifice))
            return -8;
        
        //if we have the IdealHost buff, we will not use this solver
        if (SpellsDefine.Enshroud.GetSpell().Cooldown.TotalMilliseconds != 0 && 
            SpellsDefine.Enshroud.GetSpell().Cooldown.TotalMilliseconds <= 1000 && 
            (Core.Resolve<JobApi_Reaper>().ShroudGauge >= 50 || Core.Me.HasAura(AurasDefine.IdealHost)))
            return -9;

        //if we are going to use the PlentyfulHarvest skill, we will not use this solver
        if (Core.Me.HasAura(AurasDefine.ImmortalSacrifice))
            return -10;

        
        if (ReaperSettings.Instance.DoubleEnshroud)
        {
            if(Core.Me.HasAura(AurasDefine.ArcaneCircle))
                return -1;
        }

        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}