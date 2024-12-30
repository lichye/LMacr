using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;

namespace LM.Reaper.SlotResolvers.GCD;

public class GCD_AOE:ISlotResolver
{
    //In this slot resolver，we will use one of the following skills:
    // SpinningScythe
    // NightmareScythe Lv.45
    private Spell GetSpell()
    {
        
        if (Core.Resolve<MemApiSpell>().GetLastComboSpellId() == SpellsDefine.SpinningScythe
            && Core.Me.Level >= 45)
            return SpellsDefine.NightmareScythe.GetSpell();
        return SpellsDefine.SpinningScythe.GetSpell();

    }
    public int Check()
    {   
        //Level Check
        if(Core.Me.Level < 25)
            return -20;
        
        //Target touchable check
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) >
            SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -1;
            
        //check if we have used BloodStalk or Gluttony in the last 1.5 seconds
        if (SpellsDefine.BloodStalk.RecentlyUsed(1500) || SpellsDefine.Gluttony.RecentlyUsed(1500))
            return -2;

        //AOE Check
        var aoeCount = TargetHelper.GetNearbyEnemyCount(Core.Me, 5, 5);
        if (aoeCount < 3 || !ReaperRotationEntry.QT.GetQt(QTKey.AOE))
            return -3;

        //Normal Use
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}