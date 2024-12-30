using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using static System.Windows.Forms.Design.AxImporter;
using AEAssist.JobApi;
using LM.Reaper.Setting;
namespace LM.Reaper.SlotResolvers.OffGCD;

public class oGCD_Lemure : ISlotResolver
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
        // Level Check
        if (Core.Me.Level < 80)
            return -1;
        
        // Skill Ready Check
        if (Core.Resolve<JobApi_Reaper>().VoidShroud < 2)
            return -2;
        
        // Status Check
        if (!Core.Me.HasAura(AurasDefine.Enshrouded))
            return -3;
        
        // GCD confiction Check
        if (GCDHelper.GetGCDCooldown() < ReaperSettings.Instance.AnimationLock)
            return -4;
        
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}