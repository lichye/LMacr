using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.Extension;
using AEAssist.Helper;

namespace LM.Reaper;

public static class ReaperSpellHelper
{
    public static Spell BeforeBattle()
    {
        if (!Core.Me.HasAura(AurasDefine.Soulsow))
            return SpellsDefine.Soulsow.GetSpell();
        else
            return null;
    }

}