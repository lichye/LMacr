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

public class oGCD_Enshroud : ISlotResolver
{
    private Spell GetSpell()
    {
        return SpellsDefine.Enshroud.GetSpell();
    }
    public int Check()
    {   
        if(!ReaperRotationEntry.QT.GetQt(QTKey.Enshroud))
            return -1;

        // Check if the skill is available
        if (!SpellsDefine.Enshroud.IsReady())
            return -2;
        
        // Check if we can touch the target
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) >
            SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -3;
        
        // Check if we have the Soul Reaver or Enshrouded buff or Executioner
        if (Core.Me.HasAura(AurasDefine.SoulReaver) || Core.Me.HasAura(AurasDefine.Enshrouded) || Core.Me.HasAura(AurasDefine.Executioner))
            return -4;

        // Check if we have the Ideal Host buff
        if (Core.Resolve<JobApi_Reaper>().ShroudGauge < 50 && !Core.Me.HasAura(AurasDefine.IdealHost))
            return -5;

        if (GCDHelper.GetGCDCooldown() < 600)
            return -6;

        // If we have the idealHost buff and we has skill ready, 
        // then we should use enshroud as soon as possible
        if (Core.Me.HasAura(AurasDefine.IdealHost))
            return 1;
        
        if (
            ReaperSettings.Instance.DoubleEnshroud &&
            SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds<7000
            )
            return 2;
        

        if (
            ReaperSettings.Instance.AutoEnshroud &&
            Core.Resolve<JobApi_Reaper>().ShroudGauge >= ReaperSettings.Instance.Enshroud_threadhold &&
            SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds > (100-Core.Resolve<JobApi_Reaper>().ShroudGauge)*1000+7000 &&
            SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds > 21000 &&
            SpellsDefine.Gluttony.GetSpell().Cooldown.TotalMilliseconds > 10000
            )
            return 3;

        //Normal, we will not use the enshroud
        return -100;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}