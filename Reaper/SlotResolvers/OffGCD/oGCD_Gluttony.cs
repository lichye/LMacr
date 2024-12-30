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
        //Level Check
        if (Core.Me.Level < 76)
            return -1;
        
        //Skill Ready Check
        if (Core.Resolve<JobApi_Reaper>().SoulGauge < 50 ||!SpellsDefine.Gluttony.IsReady())
            return -2;

        //GCD confiction Check
        if (GCDHelper.GetGCDCooldown() < 600)
            return -3;
        
        // Target Distance Check
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) >
            SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -3;

        // DeathsDesign Check
        if (!Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.DeathsDesign,
            ReaperSettings.Instance.GCD_Time*2+ReaperSettings.Instance.AnimationLock*3))
            return -6;
        
        // Buff confiction Check
        if (Core.Me.HasAura(AurasDefine.Enshrouded)||
            Core.Me.HasAura(AurasDefine.SoulReaver)||
            Core.Me.HasAura(AurasDefine.ImmortalSacrifice)||
            Core.Me.HasAura(AurasDefine.IdealHost))
            return -7;
        
        // Double Enshroud Check
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