using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using AEAssist.JobApi;
using System;
using System.Collections.Generic;
namespace LM.Reaper;


public class DoubleEnshroundSequence : ISlotSequence
{
    public Action CompeltedAction { get; set; }

    public int StartCheck()
    {
        if (Core.Me.Level != 100)
            return -3;
        if (SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds > 5500)
            return -5;
        if (!SpellsDefine.Enshroud.IsReady())
            return -1;
        if (ReaperRotationEntry.QT.GetQt(QTKey.Burst) == false)
            return -2;
        if (Core.Resolve<JobApi_Reaper>().ShroudGauge < 50)
            return -1;
        if (Core.Me.HasAura(AurasDefine.SoulReaver))
            return -2;
        if (Core.Me.HasAura(AurasDefine.Enshrouded))
            return -101;
        return 0;
    }

    public int StopCheck(int index)
    {
        return -1;
    }

    public List<Action<Slot>> Sequence { get; } = new()
    {
        Step0,
        Step1,
        Step2,
    };

    private static void Step0(Slot slot)
    {
        slot.Add(new Spell(SpellsDefine.Enshroud, SpellTargetType.Self));
        slot.Add(new Spell(SpellsDefine.ShadowOfDeath, SpellTargetType.Target));

    }

    private static void Step1(Slot slot)
    {
        if (Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.DeathsDesign, 40000) && Core.Me.HasAura(AurasDefine.Soulsow))
            slot.Add(new Spell(SpellsDefine.HarvestMoon, SpellTargetType.Target));
        else
            slot.Add(new Spell(SpellsDefine.ShadowOfDeath, SpellTargetType.Target));
        slot.Add(new SlotAction(SlotAction.WaitType.WaitForSndHalfWindow, 0, Spell.CreatePotion()));

    }
    private static void Step2(Slot slot)
    {
        slot.Add(new Spell(SpellsDefine.VoidReaping, SpellTargetType.Target));
        slot.Add(new Spell(SpellsDefine.ArcaneCircle, SpellTargetType.Self));
    }

}