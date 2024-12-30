using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;

namespace LM.Reaper.SlotResolvers.GCD;

public class GCD_Base : ISlotResolver
{
    //In this slot resolver，we will use one of the following skills:
    // Slice
    // WaxingSlice lv.5
    // InfernalSlice lV.30
    private Spell GetSpell()
    {
        if (Core.Resolve<MemApiSpell>().GetLastComboSpellId() == SpellsDefine.WaxingSlice 
            && Core.Me.Level >= 30)
            return SpellsDefine.InfernalSlice.GetSpell();
        
        if (Core.Resolve<MemApiSpell>().GetLastComboSpellId() == SpellsDefine.Slice
            && Core.Me.Level >= 5)
            return SpellsDefine.WaxingSlice.GetSpell();
        
        return SpellsDefine.Slice.GetSpell();
    }
    public int Check()
    {   
        //Target touchable check
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) >
            SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -1;

       // Buff confiction Check
        if (Core.Me.HasAura(AurasDefine.Enshrouded)||
            Core.Me.HasAura(AurasDefine.SoulReaver)||
            Core.Me.HasAura(AurasDefine.Executioner)||
            Core.Me.HasAura(AurasDefine.ImmortalSacrifice)||
            Core.Me.HasAura(AurasDefine.IdealHost))
            return -7;
        
        //Normal Use
        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}