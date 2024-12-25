using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using AEAssist.JobApi;

namespace LM.Reaper.SlotResolvers.GCD;

public class SlotResolver_GCD_Reaping : ISlotResolver
{
    // In the current slot, the skill is added to the slot
    // Void Reaping
    // Soul Reaping
    private Spell GetSpell()
    {
        if (Core.Me.HasAura(AurasDefine.EnhancedCrossReaping))
            return SpellsDefine.CrossReaping.GetSpell();
        return SpellsDefine.VoidReaping.GetSpell();
    }

    public int Check()
    {
        // Check if we can touch the target
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) >
            SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -1;
        // Check we have the skill
        if (Core.Me.Level < 80)
            return -2;

        // if we are not in the enshrouded state, don't use it
        if (!Core.Me.HasAura(AurasDefine.Enshrouded))
            return -3;
       
        //TODO:
        //Add more checks here
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}