using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;

using LM.Reaper.Setting;

namespace LM.Reaper.SlotResolvers.OffGCD;

public class oGCD_ArcaneCircle : ISlotResolver
{
    private Spell GetSpell()
    {
        return SpellsDefine.ArcaneCircle.GetSpell();
    }
    public int Check()
    {   
        // Level Check
        if(Core.Me.Level < 72)
            return -1;

        // Non-conflict with GCD check
        if (GCDHelper.GetGCDCooldown() < ReaperSettings.Instance.AnimationLock)
            return -2;

        //Skill Ready Check
        if (!SpellsDefine.ArcaneCircle.IsReady())
            return -2;

        //If Standard Shroud is enabled, check if the GCD is ready
        if(ReaperSettings.Instance.StandardShroud && GCDHelper.GetGCDCooldown()< 2*ReaperSettings.Instance.AnimationLock)
            return -1;

        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}