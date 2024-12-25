using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;

namespace LM.Reaper.SlotResolvers.OffGCD;

public class SlotResolver_OffGCD_ArcaneCircle : ISlotResolver
{
    public int Check()
    {

        if (!SpellsDefine.ArcaneCircle.IsReady())
            return -1;
        // if (ReaperRotationEntry.QT.GetQt(QTKey.Burst) == false)
        // return -2;

        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(SpellsDefine.ArcaneCircle.GetSpell().Id).GetSpell());
    }
}