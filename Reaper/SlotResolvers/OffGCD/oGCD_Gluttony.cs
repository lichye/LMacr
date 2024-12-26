using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using static System.Windows.Forms.Design.AxImporter;
using AEAssist.JobApi;

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
            return -20;
        
        //if we don't have the SoulGauge, we will not use this solver
        if (Core.Resolve<JobApi_Reaper>().SoulGauge < 50)
            return -11;
        
        //if we can't use GCD, we will not use this solver
        if (GCDHelper.GetGCDCooldown() < 600)
            return -2;
        
        //if we do not open the Gluttony QT, we will not use this solver
        // if (ReaperRotationEntry.QT.GetQt(QTKey.Gluttony) == false)
        //     return -13;
        
        //if the Gluttony skill is not ready, we will not use this solver
        if (!SpellsDefine.Gluttony.IsReady())
            return -12;
        
        //if the target does not have the DeathsDesign debuff, we will not use this solver
        if (!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.DeathsDesign, 5000))
            return -6;
        
        //if we have the Enshrouded buff, we will not use this solver
        if (Core.Me.HasAura(AurasDefine.Enshrouded))
            return -51;
        
        
        if (Core.Me.HasAura(AurasDefine.ImmortalSacrifice))
            return -3;
        if (SpellsDefine.Enshroud.GetSpell().Cooldown.TotalMilliseconds != 0 && 
            SpellsDefine.Enshroud.GetSpell().Cooldown.TotalMilliseconds <= 1000 && 
        // ReaperRotationEntry.QT.GetQt(QTKey.Burst) != false 
        //  && ReaperRotationEntry.QT.GetQt(QTKey.Enshroud) != false &&

            (Core.Resolve<JobApi_Reaper>().ShroudGauge >= 50 || Core.Me.HasAura(AurasDefine.IdealHost)))
            return -4;
        if (Core.Me.HasAura(AurasDefine.ArcaneCircle) && SpellsDefine.Enshroud.IsReady())
            return -52;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}