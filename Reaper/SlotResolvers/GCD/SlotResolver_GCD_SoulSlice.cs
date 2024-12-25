using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using AEAssist.JobApi;

namespace LM.Reaper.SlotResolvers.GCD;

public class SlotResolver_GCD_SoulSlice : ISlotResolver
{
    // In the current slot, the skill is added to the slot
    // SouldScythe Lv.65
    // SoulSlice Lv.60
    private Spell GetSpell()
    {
        if (ReaperRotationEntry.QT.GetQt(QTKey.AOE))

        {
            var aoeCount = TargetHelper.GetNearbyEnemyCount(Core.Me, 5, 5);
            if (aoeCount >= 3)
                return SpellsDefine.SoulScythe.GetSpell();
        }

        return SpellsDefine.SoulSlice.GetSpell();
    }

    public int Check()
    {
        // Check if the skill is available
        if (!SpellsDefine.SoulSlice.IsReady())
            return -1;

        // Check if we have too much Soul Gauge
        if (Core.Resolve<JobApi_Reaper>().SoulGauge > 50)
            return -2;
        
        // Check if we have the Soul Reaver buff
        if (Core.Me.HasAura(AurasDefine.SoulReaver))
            return -3;
        
        // Check if we can touch the target
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) >
            SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -4;
        
        // if we will break the combo, don't use it
        if (Core.Resolve<MemApiSpell>().GetComboTimeLeft().TotalMilliseconds < 5000)
            if (Core.Resolve<MemApiSpell>().GetLastComboSpellId() == SpellsDefine.Slice ||
                Core.Resolve<MemApiSpell>().GetLastComboSpellId() == SpellsDefine.WaxingSlice)
                return -5;
        //TODO:
        //Add more checks here
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}