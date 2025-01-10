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
        //if Battletime is too long, then we do not need the opener
        if(AI.Instance.BattleData.CurrBattleTimeInMs < (ReaperSettings.Instance.ArcaneCircle_GCD-1)*ReaperSettings.Instance.GCD_Time-ReaperSettings.Instance.AnimationLock)
            return -100;

        // Turn off the burst mode
        if(!ReaperRotationEntry.QT.GetQt(QTKey.Burst))
            return -1;
            
        // Level Check
        if(Core.Me.Level < 72)
            return -1;

        // Non-conflict with GCD check
        if (GCDHelper.GetGCDCooldown() < ReaperSettings.Instance.AnimationLock)
            return -2;

         
        // // Last oGCD check
        // if (!SpellsDefine.Gluttony.IsReady()&&ReaperSettings.Instance.DoubleEnshroud &&GCDHelper.GetGCDCooldown()> ReaperSettings.Instance.AnimationLock*2)
        //     return -4;
        
        // Standard Shroud Check
        if (ReaperSettings.Instance.StandardShroud&& GCDHelper.GetGCDCooldown()< ReaperSettings.Instance.AnimationLock*1.2)
            return -3;
        
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