using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;

namespace LM.Reaper.SlotResolvers.GCD;

public class SlotResolver_GCD_PlentifulHarvest : ISlotResolver
{
    
    public int Check()
    {
        // if (!ReaperRotationEntry.QT.GetQt(QTKey.PlentifulHarvest))
        //     return -4;
        if (!SpellsDefine.PlentifulHarvest.IsUnlock())
            return -1;
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) > 15)
            return -1;
        if (Core.Me.HasAura(AurasDefine.BloodsownCircle))
            return -2;
        if (Core.Me.HasAura(AurasDefine.SoulReaver))
            return -2;
        if (!Core.Me.HasAura(AurasDefine.ImmortalSacrifice))
            return -3;
        return 0;
    }
    
    public void Build(Slot slot)
    {
        slot.Add(SpellsDefine.PlentifulHarvest.GetSpell());
    }
}