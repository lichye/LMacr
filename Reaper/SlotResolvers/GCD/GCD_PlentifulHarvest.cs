using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;

namespace LM.Reaper.SlotResolvers.GCD;

public class GCD_PlentifulHarvest : ISlotResolver
{
    
    public int Check()
    {
        //if we are below level 88, we will not use this solver
        if (Core.Me.Level < 88)
            return -1;

        //if we don't have the SoulGauge/Executioner, we will not use this solver
        if (Core.Me.HasAura(AurasDefine.SoulReaver) || Core.Me.HasAura(AurasDefine.Executioner))
            return -2;

        //if we cannot touch the target, we will not use this solver
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) >SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -3;

        //if we have the BloodsownCircle, we will not use this solver
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) > 15)
            return -4;
        
        if (Core.Me.HasAura(AurasDefine.BloodsownCircle))
            return -5;
        
        if (!Core.Me.HasAura(AurasDefine.ImmortalSacrifice))
            return -7;
        
        if(Core.Me.HasAura(AurasDefine.Enshrouded))
            return -8;

        return 0;
    }
    
    public void Build(Slot slot)
    {
        slot.Add(SpellsDefine.PlentifulHarvest.GetSpell());
    }
}