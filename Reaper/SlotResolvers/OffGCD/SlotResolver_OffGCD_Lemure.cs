using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using static System.Windows.Forms.Design.AxImporter;
using AEAssist.JobApi;

namespace LM.Reaper.SlotResolvers.OffGCD;

public class SlotResolver_OffGCD_Lemure : ISlotResolver
{
    public int Check()
    {
        if (Core.Resolve<JobApi_Reaper>().VoidShroud < 2)
            return -2;
        if (!Core.Me.HasAura(AurasDefine.Enshrouded))
            return -3;
        if (GCDHelper.GetGCDCooldown() < 800)
            return -2;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(Core.Resolve<MemApiSpell>().CheckActionChange(SpellsDefine.LemuresSlice.GetSpell().Id).GetSpell());
    }
}