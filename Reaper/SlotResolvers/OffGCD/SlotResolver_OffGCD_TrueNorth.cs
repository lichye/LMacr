using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using static System.Windows.Forms.Design.AxImporter;

namespace LM.Reaper.SlotResolvers.OffGCD;

public class SlotResolver_OffGCD_TrueNorth : ISlotResolver
{

    public int Check()
    {
        if (GCDHelper.GetGCDCooldown() < 600)
            return -2;
        // if (!ReaperRotationEntry.QT.GetQt(QTKey.UseTrueNorth))//真北开关
        // {
        //     return -10;
        // }
        if (!SpellsDefine.TrueNorth.IsReady())//真北CD好了没
        {
            return -9;
        }
        if (Core.Me.HasAura(AurasDefine.TrueNorth) || SpellsDefine.TrueNorth.RecentlyUsed())//真北BUFF
        {
            return -8;
        }


        var t1 = (Core.Resolve<MemApiSpellCastSuccess>().LastGcdSuccesTime + 2000 - TimeHelper.Now()) < 2000;
        //LogHelper.Info($"需要放真北吗？：{Core.Me.GetCurrTarget().HasPositional() && t1}，背：{Core.Me.HasMyAura(AurasDefine.EnhancedWheelingThrust) && !Core.Me.GetCurrTarget().IsBehind}");
        if (Core.Me.GetCurrTarget().HasPositional() && t1)//如果目标有身位
        {
            // 
            if (Core.Me.HasAura(AurasDefine.EnhancedGallows) && Core.Me.HasAura(AurasDefine.SoulReaver) && !Core.Resolve<MemApiTarget>().IsBehind)
            {
                return 0;
            }
            // 
            if (Core.Me.HasAura(AurasDefine.EnhancedGibbet) && Core.Me.HasAura(AurasDefine.SoulReaver) && !Core.Resolve<MemApiTarget>().IsFlanking)
            {
                return 0;
            }
        }
        return -1;
    }

    public void Build(Slot slot)
    {
        slot.Add(SpellsDefine.TrueNorth.GetSpell());
    }
}