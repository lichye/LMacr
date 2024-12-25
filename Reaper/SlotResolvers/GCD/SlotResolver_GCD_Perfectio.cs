using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;

namespace LM.Reaper.SlotResolvers.GCD;

public class SlotResolver_GCD_Perfectio : ISlotResolver
{
    public int Check()
    {
        if (!SpellsDefine.Perfectio.IsUnlock())
            return -1;
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) > 25)
            return -1;
        if (!Core.Me.HasAura(AurasDefine.PerfectioParata))
            return -3;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(SpellsDefine.Perfectio.GetSpell());
    }
}