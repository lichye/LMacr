using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using static System.Windows.Forms.Design.AxImporter;
using LM.Reaper.Setting;

namespace LM.Reaper.SlotResolvers.OffGCD;

public class oGCD_TrueNorth : ISlotResolver
{
    //In this slot resolver，we will use one of the following skills:
    // TrueNorth Lv.0
    private Spell GetSpell()
    {
        return SpellsDefine.TrueNorth.GetSpell();
    }

    public int Check()
    {
        // Turn off auto TrueNorth
        if(!ReaperRotationEntry.QT.GetQt(QTKey.AutoTrueNorth))
            return -1;

        //GCD confiction Check
        if (GCDHelper.GetGCDCooldown() < 600)
            return -2;

        //Skill Ready Check
        if (!SpellsDefine.TrueNorth.IsReady())//真北CD好了没
        {
            return -3;
        }

        if (!Core.Me.GetCurrTarget().HasPositional())
        {
            return -4;
        }

        //Position check
        if(
            Core.Resolve<JobApi_Reaper>().SoulGauge == 100 ||
            Core.Me.HasAura(AurasDefine.SoulReaver)||
            Core.Me.HasAura(AurasDefine.Executioner)){
            //still have time to use TrueNorth
            if(GCDHelper.GetGCDCooldown() > ReaperSettings.Instance.AnimationLock*1.2)
            {
                return -5;
            }
            //if we are on the back of the Target and we have the aura of EnhancedGibbet, we will not use this solver
            if (Core.Me.HasAura(AurasDefine.EnhancedGibbet) && !Core.Resolve<MemApiTarget>().IsFlanking && !Core.Me.HasAura(AurasDefine.TrueNorth))
            {
                return 1;
            }

            //if we are on the side of the Target and we have the aura of EnhancedGallows, we will not use this solver
            if (Core.Me.HasAura(AurasDefine.EnhancedGallows) && !Core.Resolve<MemApiTarget>().IsBehind && !Core.Me.HasAura(AurasDefine.TrueNorth))
            {
                return 1;
            }
        }

        //Normally we will not use TrueNorth
        return -100;
    }

    public void Build(Slot slot)
    {
        slot.Add(SpellsDefine.TrueNorth.GetSpell());
    }
}