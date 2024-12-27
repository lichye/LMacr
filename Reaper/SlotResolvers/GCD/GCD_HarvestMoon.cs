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
        //if we do not have HarvestMoon, we will not use this solver
        if (!Core.Me.HasAura(AurasDefine.Soulsow))
            return -1;
    
        //if we have Excuter buff, we will not use this solver
        if (Core.Me.HasAura(AurasDefine.Executioner)||Core.Me.HasAura(AurasDefine.SoulReaver))
            return -2;
        
        //if turn off the QT, then we will not use this solver
        if (!ReaperRotationEntry.QT.GetQt(QTKey.HarvestMoon))
            return -3;
        
        //if we are below level 82, we will not use this solver
        if (Core.Me.Level < 82)
            return -3;

        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}