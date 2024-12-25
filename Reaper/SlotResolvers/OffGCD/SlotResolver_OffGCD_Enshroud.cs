using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using static System.Windows.Forms.Design.AxImporter;
using AEAssist.JobApi;

namespace LM.Reaper.SlotResolvers.OffGCD;

public class SlotResolver_OffGCD_Enshroud : ISlotResolver
{
    private Spell GetSpell()
    {
        return SpellsDefine.Enshroud.GetSpell();
    }
    public int Check()
    {   
        // Check if the skill is available
        if (!SpellsDefine.Enshroud.IsReady())
            return -1;
        
        // Check if we can touch the target
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) >
            SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -2;
        
        // Check if we have the Soul Reaver or Enshrouded buff or Executioner
        if (Core.Me.HasAura(AurasDefine.SoulReaver) || Core.Me.HasAura(AurasDefine.Enshrouded) || Core.Me.HasAura(AurasDefine.Executioner))
            return -3;

        // Check if we have the Ideal Host buff
        if (Core.Resolve<JobApi_Reaper>().ShroudGauge < 50 && !Core.Me.HasAura(AurasDefine.IdealHost))
            return -4;

        /*if (RPRSettings.Instance.EnshroundPotion3 && RPRSettings.Instance.DoubleEnshroundPotion &&
            Core.Resolve<MemApiInventory>().GetItemCount(SettingMgr.GetSetting<PotionSetting>().GetPotionId(Core.Me.CurrentJob), true) != 0 &&
            Core.Resolve<MemApiInventory>().GetItemCoolDown(SettingMgr.GetSetting<PotionSetting>().GetPotionId(Core.Me.CurrentJob)).TotalMilliseconds 
            <= SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds)
            return -9;*/
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}