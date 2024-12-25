using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using AEAssist.JobApi;

namespace LM.Reaper.SlotResolvers.GCD;

public class SlotResolver_GCD_Enshroud : ISlotResolver
{
    private Spell GetSpell()
    {
        if (!Core.Me.HasAura(AurasDefine.Enshrouded))
            return null;

        if (Core.Resolve<JobApi_Reaper>().LemureShroud < 2
            && SpellsDefine.Communio.IsUnlock())
            return SpellsDefine.Communio.GetSpell();

        if (Core.Me.HasAura(AurasDefine.EnhancedVoidReaping))
            return SpellsDefine.VoidReaping.GetSpell();

        if (Core.Me.HasAura(AurasDefine.EnhancedCrossReaping))
            return SpellsDefine.CrossReaping.GetSpell();


        if (SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds <= 5000 && ReaperRotationEntry.QT.GetQt(QTKey.Burst) == true )
        return SpellsDefine.ShadowOfDeath.GetSpell(); ;

        return SpellsDefine.VoidReaping.GetSpell();
    }
    // 返回>=0表示检测通过 即将调用Build方法
    public int Check()
    {
        if (Core.Resolve<JobApi_Reaper>().LemureShroud == 0)
            return -4;
        if (!Core.Me.HasAura(AurasDefine.Enshrouded))
            return -2;

        return 0;
    }

    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}