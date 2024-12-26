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
        //if we can not touch the target, we will not use this solver
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) >
            SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -1;

        //if we have used BloodStalk or Gluttony in the last 1.5 seconds, we will not use this solver
        if (SpellsDefine.BloodStalk.RecentlyUsed(1500) || SpellsDefine.Gluttony.RecentlyUsed(1500))
            return -2;
        
        //if we have Excuter buff, we will not use this solver
        if (Core.Me.HasAura(AurasDefine.Executioner)||Core.Me.HasAura(AurasDefine.SoulReaver))
            return -3;
        return 1;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}