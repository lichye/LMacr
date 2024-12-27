using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using AEAssist.JobApi;

namespace LM.Reaper.SlotResolvers.GCD;

public class GCD_Reaping : ISlotResolver
{
    private Spell GetSpell()
    {
        if (Core.Resolve<JobApi_Reaper>().LemureShroud < 2
            && SpellsDefine.Communio.IsUnlock())
            return SpellsDefine.Communio.GetSpell();

        if (Core.Me.HasAura(AurasDefine.EnhancedVoidReaping))
            return SpellsDefine.VoidReaping.GetSpell();

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

        if (Core.Resolve<JobApi_Reaper>().LemureShroud == 0)
            return -2;
        
        if (!Core.Me.HasAura(AurasDefine.Enshrouded))
            return -3;

        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}