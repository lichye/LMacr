using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;

namespace LM.Reaper.SlotResolvers.GCD;

public class GCD_HarvestMoon : ISlotResolver
{
    //In this slot resolver，we will use one of the following skills:
    // HarvestMoon Lv.82
    private Spell GetSpell()
    {
        return SpellsDefine.HarvestMoon.GetSpell();
    }
    public int Check()
    {   
        //Level Check
        if (Core.Me.Level < 82)
            return -3;

        //QT Check
        if (!ReaperRotationEntry.QT.GetQt(QTKey.HarvestMoon))
            return -3;

        //Skill Ready Check
        if (!Core.Me.HasAura(AurasDefine.Soulsow))
            return -1;
    
        // Buff confiction Check
        if (Core.Me.HasAura(AurasDefine.Enshrouded)||
            Core.Me.HasAura(AurasDefine.SoulReaver)||
            Core.Me.HasAura(AurasDefine.ImmortalSacrifice)||
            Core.Me.HasAura(AurasDefine.IdealHost))
            return -7;
        
        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}