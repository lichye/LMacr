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

public class oGCD_Gemdraught : ISlotResolver
{
    // In this slot resolver，we will use one of the following skills:
    // Gemdraught
    private Spell GetSpell()
    {
        return Spell.CreatePotion();
    }
    public int Check()
    {   
        if(!ReaperRotationEntry.QT.GetQt(QTKey.EnableGemdraught))
            return -1;
    
        if (!ItemHelper.CheckCurrJobPotion())
            return -100;
        if (!SpellsDefine.Potion.IsReady())
            return -1;
        if (GCDHelper.GetGCDCooldown() < 1000)
            return -2;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(GetSpell());
    }
}