using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using static System.Windows.Forms.Design.AxImporter;
using AEAssist.JobApi;

namespace LM.Reaper.SlotResolvers.OffGCD;

public class SlotResolver_OffGCD_Lemure : ISlotResolver
{
    private Spell GetSpell()
    {
        var aoeCount = TargetHelper.GetNearbyEnemyCount(Core.Me, 5, 5);
        if (aoeCount >= 3 || ReaperRotationEntry.QT.GetQt(QTKey.AOE))
            return SpellsDefine.LemuresScythe.GetSpell();
        return SpellsDefine.LemuresSlice.GetSpell();
    }
    public int Check()
    {
        if (Core.Resolve<JobApi_Reaper>().VoidShroud < 2)
            return -2;
        if (!Core.Me.HasAura(AurasDefine.Enshrouded))
            return -3;
        if (GCDHelper.GetGCDCooldown() < 800)
            return -2;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}