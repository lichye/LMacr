using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;

namespace LM.Reaper.SlotResolvers.OffGCD;

public class oGCD_ArcaneCircle : ISlotResolver
{
    private Spell GetSpell()
    {
        return SpellsDefine.ArcaneCircle.GetSpell();
    }
    public int Check()
    {   
        // Check if the skill is available
        if(Core.Me.Level < 72)
            return -1;
        
        if (!SpellsDefine.ArcaneCircle.IsReady())
            return -2;

        // if (ReaperRotationEntry.QT.GetQt(QTKey.Burst) == false)
        // return -2;

        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}