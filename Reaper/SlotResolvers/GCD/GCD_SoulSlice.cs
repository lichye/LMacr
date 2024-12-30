using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using AEAssist.JobApi;

namespace LM.Reaper.SlotResolvers.GCD;

public class GCD_SoulSlice : ISlotResolver
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
        // Level Check
        if (Core.Me.Level < 60)
            return -1;
        
        // Skill Ready Check
        if (!SpellsDefine.SoulSlice.IsReady())
            return -1;

        // SoulGauge Overflow Check
        if (Core.Resolve<JobApi_Reaper>().SoulGauge > 50)
            return -2;
        
        // Buff confiction Check
        if (Core.Me.HasAura(AurasDefine.SoulReaver)||
            Core.Me.HasAura(AurasDefine.Executioner))
            return -3;
        
        // Gluttony Check
        if (SpellsDefine.Gluttony.CoolDownInGCDs(3) && Core.Resolve<JobApi_Reaper>().SoulGauge ==50)
            return -4;

        // Target touchable check
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) >
            SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -4;

        // Gluttony Check

        
        // Combo Time Check
        if (Core.Resolve<MemApiSpell>().GetComboTimeLeft().TotalMilliseconds < 5000)
            if (Core.Resolve<MemApiSpell>().GetLastComboSpellId() == SpellsDefine.Slice ||
                Core.Resolve<MemApiSpell>().GetLastComboSpellId() == SpellsDefine.WaxingSlice)
                return -5;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}