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
        //Skill Ready Check
        if (!SpellsDefine.ArcaneCircle.IsReady())
            return -2;

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
        
        // Standard Shroud Check
        if (ReaperSettings.Instance.StandardShroud){
            //if time is limited, then we do not need the opener
            if (GCDHelper.GetGCDCooldown() < ReaperSettings.Instance.AnimationLock*1.2)
                return -3;
            if (!SpellsDefine.Gluttony.CoolDownInGCDs(1))
                return -4;
            //if it is not match up with Gluttony, then we do not need the opener
        }
        


        // if we are going to DoubleEnshroud, then we might make the ArcaneCircle later
        if(ReaperBattleData.Instance.AutoDoubleEnshroud)
            return -100;{
        }

        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}