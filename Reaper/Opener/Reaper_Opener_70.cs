using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using LM.Reaper.Setting;
using System;
using System.Collections.Generic;

namespace LM.Reaper.Opener;

public class Reaper_Opener_70 : IOpener
{
    public int StartCheck()
    {   
        
        //if Battletime is too long, then we do not need the opener
        if(AI.Instance.BattleData.CurrBattleTimeInMs > 5000)
            return -1;

        //if ArcaneCircle is ready
        if  (!SpellsDefine.ArcaneCircle.IsReady())
            return -1;
        
        //if we are Enshrouded,then we do not need the opener
        if (Core.Me.HasAura(AurasDefine.Enshrouded))
            return -1;
        
        //if we have enough ShroudGauge, then we do not need the opener
        if (Core.Resolve<JobApi_Reaper>().ShroudGauge >=50)
            return -1;
        
        //Normal Mode Check
        if(ReaperSettings.Instance.Normal)
            return -1;
        
        
        return 0;
    }

    public int StopCheck(int index)
    {   
        //if we are Enshrouded,then we do not need the opener
        if(Core.Me.HasAura(AurasDefine.Enshrouded))
            return 0;
        return -1;
    }

    public List<Action<Slot>> Sequence { get; } = new()
    {
        GCD_1,
    };

    public Action CompeltedAction { get; set; }

    public int StepCount => 0;

    private static void GCD_1(Slot slot)
    {
        slot.Add(new Spell(SpellsDefine.ShadowOfDeath, SpellTargetType.Target));
        // slot.Add(Spell.CreatePotion());
    }
    public uint Level { get; } = 70;

    public void InitCountDown(CountDownHandler countDownHandler)
    {
        countDownHandler.AddAction(5000, ReaperSpellHelper.BeforeBattle);
        if(ReaperSettings.Instance.PreHarpe)
            countDownHandler.AddAction(ReaperSettings.Instance.Harpe_time, SpellsDefine.Harpe, SpellTargetType.Target);
    }
}