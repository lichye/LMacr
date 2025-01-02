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

public class oGCD_Sacrificium : ISlotResolver
{
    public int Check()
    {
        // Level Check
        if (Core.Me.Level < 92)
            return -1;
        
        // GCD confiction Check
        if (GCDHelper.GetGCDCooldown() < ReaperSettings.Instance.AnimationLock)
            return -2;
        
        // Skill Ready Check
        if (!Core.Me.HasAura(AurasDefine.Oblatio)||!SpellsDefine.Sacrificium.IsReady())
            return -5;
        
        // DeathsDesign Check
        if (!Core.Me.GetCurrTarget().HasAura(AurasDefine.DeathsDesign))
            return -3;
        
        // ArcaneCircle Check
        if (SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds < 10000)
            return -4;
        
        // ArcaneCircle buff Check
        if (SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds > 110000 &&
            !Core.Me.HasAura(AurasDefine.ArcaneCircle))
            return -6;

        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(SpellsDefine.Sacrificium.GetSpell());
    }
}