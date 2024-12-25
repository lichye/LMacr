using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using LM.Reaper.Setting;
using System;
using System.Collections.Generic;

namespace LM.Reaper.Opener;

public class Reaper_Opener100 : IOpener
{
    public int StartCheck()
    {
        if (Core.Resolve<MemApiSpell>().GetCharges(SpellsDefine.SoulSlice) < 2)
            return -13;
        if (!SpellsDefine.ArcaneCircle.IsReady())
            return -1;
        if (!SpellsDefine.SoulSlice.IsMaxChargeReady(0.1f))
            return -2;
        if (!SpellsDefine.Gluttony.CoolDownInGCDs(7))
            return -3;
        if (!SpellsDefine.Enshroud.IsReady())
            return -4;
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
        Step3,
        Step4
    };

    public Action CompeltedAction { get; set; }


    public int StepCount => 6;

    private static void Step0(Slot slot)
    {
        ChatHelper.Print.ErrorMessage("0");
        slot.Add(new Spell(SpellsDefine.ShadowOfDeath, SpellTargetType.Target));
        if (
            // ReaperRotationEntry.QT.GetQt(QTKey.UsePotion) == true && 
            !ReaperSettings.Instance.OpenerNoPos)
        {
            slot.Add(Spell.CreatePotion());
        }
    }


    private static void Step1(Slot slot)
    {
        ChatHelper.Print.ErrorMessage("1");
        slot.Add(new Spell(SpellsDefine.SoulSlice, SpellTargetType.Target));
        slot.Add(new Spell(SpellsDefine.ArcaneCircle, SpellTargetType.Self));
        slot.Add(new Spell(SpellsDefine.Gluttony, SpellTargetType.Target));
    }


    private static void Step2(Slot slot)
    {
        if (ReaperSettings.Instance.BaseGCD_BehindFirst)
            slot.Add(new Spell(SpellsDefine.ExGallows, SpellTargetType.Target));
        else
            slot.Add(new Spell(SpellsDefine.ExGibbet, SpellTargetType.Target));

    }


    private static void Step3(Slot slot)
    {
        if (ReaperSettings.Instance.BaseGCD_BehindFirst)
            slot.Add(new Spell(SpellsDefine.ExGibbet, SpellTargetType.Target));
        else
            slot.Add(new Spell(SpellsDefine.ExGallows, SpellTargetType.Target));
    }

    private static void Step4(Slot slot)
    {
        slot.Add(new Spell(SpellsDefine.PlentifulHarvest, SpellTargetType.Target));
        slot.Add(new Spell(SpellsDefine.Enshroud, SpellTargetType.Self));
        slot.Add(new Spell(SpellsDefine.Sacrificium, SpellTargetType.Target));
    }



    public uint Level { get; } = 100;

    public void InitCountDown(CountDownHandler countDownHandler)
    {

        countDownHandler.AddAction(5000, ReaperSpellHelper.BeforeBattle);
        countDownHandler.AddAction(ReaperSettings.Instance.Harpe_time, SpellsDefine.Harpe, SpellTargetType.Target);
        ChatHelper.Print.ErrorMessage("倒计时队列");
    }
}