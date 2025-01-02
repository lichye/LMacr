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

public class oGCD_Enshroud : ISlotResolver
{
    private Spell GetSpell()
    {
        return SpellsDefine.Enshroud.GetSpell();
    }
    public int Check()
    {   
        // Level Check
        if(Core.Me.Level < 80)
            return -1;
        
         // Non-conflict with GCD check
        if (GCDHelper.GetGCDCooldown() < ReaperSettings.Instance.AnimationLock)
            return -2;
        
        // QT Check
        if(!ReaperRotationEntry.QT.GetQt(QTKey.Enshroud))
            return -1;

        // Ideal Host Trigger
        if (Core.Me.HasAura(AurasDefine.IdealHost))
            return 1;

        // Skill Ready Check
        if (!SpellsDefine.Enshroud.IsReady() || Core.Resolve<JobApi_Reaper>().ShroudGauge < 50)
            return -2;
        
        // Target Distance Check
        if (Core.Me.Distance(Core.Me.GetCurrTarget()) >
            SettingMgr.GetSetting<GeneralSettings>().AttackRange)
            return -3;
        
        // Buff confiction Check
        if (Core.Me.HasAura(AurasDefine.SoulReaver) || 
            Core.Me.HasAura(AurasDefine.Enshrouded) || 
            Core.Me.HasAura(AurasDefine.Executioner))
            return -4;

        

        // In Double Enshroud Trigger
        if (ReaperSettings.Instance.DoubleEnshroud){
            // Double Enshroud Ability Check
            if (!ReaperBattleData.Instance.IsAbleDoubleEnshroud)
                return -100;
            // preEnshroudTime Check
            if(SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds < ReaperSettings.Instance.preEnshroudTime){
                ReaperBattleData.Instance.AutoDoubleEnshroud = true;
                ReaperBattleData.Instance.DoOneReaping = true;
                return 2;
            }
                
        }

        //Below is the Auto Enshroud Mode
        //check the SoulSlice
        if(SpellsDefine.SoulSlice.IsReady()){
            return -100;
        }

        //Enough ShroudGauge Check
        if(Core.Resolve<JobApi_Reaper>().ShroudGauge < ReaperSettings.Instance.Enshroud_threadhold)
            return -100;
        
        //Gluttony conflict Check
        if(SpellsDefine.Gluttony.CoolDownInGCDs(5))
            return -100;

        //Gluttony conflict Check-2
        if(Core.Resolve<JobApi_Reaper>().SoulGauge<= 30 &&
          SpellsDefine.Gluttony.GetSpell().Cooldown.TotalMilliseconds< SpellsDefine.SoulSlice.GetSpell().Cooldown.TotalMilliseconds)
            return -100;

        // Auto Enshroud Mode
        if(!ReaperSettings.Instance.AutoEnshroud){
            return -100;    
        }

        //Immortal Sacrifice Check
        if(Core.Me.HasAura(AurasDefine.ImmortalSacrifice)){
            return -100;
        }

        //Standard Shroud Trigger
        if(ReaperSettings.Instance.StandardShroud){
            return 1;
        }

        // Auto Enshroud Trigger
        if(ReaperSettings.Instance.DoubleEnshroud){
            //we need to think whether this usage of Enshroud will lead us cannot do Double Enshroud
            int remainEnough = Core.Resolve<JobApi_Reaper>().ShroudGauge - 50;
            int ArcaneTime = (int)SpellsDefine.ArcaneCircle.GetSpell().Cooldown.TotalMilliseconds/1000;
            if(ArcaneTime*0.9+remainEnough>=50)
                return 1;
            else
                return -100;
        }
        
        //Normal, we will not use the enshroud
        return -100;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}