using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;

namespace LM.Reaper.SlotResolvers.GCD;

public class GCD_Perfectio : ISlotResolver
{
    public int Check()
    {   
        //Level Check
        if (Core.Me.Level < 100)
            return -1;
        
        //Skill Ready Check
        if (!SpellsDefine.Perfectio.IsUnlock()||!Core.Me.HasAura(AurasDefine.PerfectioParata))
            return -1;
        
        //Target touchable check
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) > 25)
            return -1;

        //Normal Use
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(SpellsDefine.Perfectio.GetSpell());
    }
}