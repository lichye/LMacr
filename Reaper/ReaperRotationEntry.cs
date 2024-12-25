
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.CombatRoutine.View.JobView.HotkeyResolver;
using ImGuiNET;
using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.Helper;

using AEAssist.Extension;

using LM.Reaper.SlotResolvers.GCD;
using LM.Reaper.SlotResolvers.OffGCD;
using LM.Reaper.Triggers;
using LM.Reaper.Opener;
using LM.Reaper.Setting;

using System;
using System.Collections.Generic;

namespace LM.Reaper;

public class ReaperRotationEntry : IRotationEntry
{   
    //public memebers
    public string AuthorName { get; set; } = "LM";

    public static JobViewWindow QT { get; private set; }

    public static ReaperBattleData RB{ get; private set; }

    // Logic is judged from top to bottom, the common queue is judged no matter what
    // gcd is judged when gcd can be used
    // offGcd is judged when gcd cannot be used and the ability skill limit within gcd is not reached
    // In the pvp environment, all are forcibly considered as common queues
    private List<SlotResolverData> SlotResolvers = new()
    {
        // gcd lists
        // the order be like：
        // shadowofdead
        // Gibbet + Gallows
        // single_target_base
        // aoe_base
        new(new SlotResolver_GCD_ShadowofDeath(),SlotMode.Gcd),

        new(new SlotResolver_GCD_Reaping(),SlotMode.Gcd),

        new(new SlotResolver_GCD_Gibbet(),SlotMode.Gcd),

        new(new SlotResolver_GCD_SoulSlice(),SlotMode.Gcd),

        new(new SlotResolver_GCD_Base(),SlotMode.Gcd),

        new(new SlotResolver_GCD_AOE_Base(),SlotMode.Gcd),
        

        // gcd队列
        // new(new SlotResolver_GCD_Perfectio(),SlotMode.Gcd),

        // new(new SlotResolver_GCD_Enshroud(),SlotMode.Gcd),
        // new(new SlotResolver_GCD_PlentifulHarvest(),SlotMode.Gcd),
        // new(new SlotResolver_GCD_Gibbet(),SlotMode.Gcd),
        // new(new SlotResolver_GCD_SoulSlice(),SlotMode.Gcd),
        
        // ogcd lists
        // the order be like：
        // Sacrificium
        // Gluttony
        // BloodStalk

        // new (new SlotResolver_OffGCD_ArcaneCircle(),SlotMode.OffGcd),
        new (new SlotResolver_OffGCD_Enshroud(),SlotMode.OffGcd),
        new (new SlotResolver_OffGCD_Sacrificium(),SlotMode.OffGcd),
        // new (new SlotResolver_OffGCD_TrueNorth(),SlotMode.OffGcd),
        // new (new SlotResolver_OffGCD_Lemure(),SlotMode.OffGcd),
        new (new SlotResolver_OffGCD_Gluttony(),SlotMode.OffGcd),
        new (new SlotResolver_OffGCD_BloodStalk(),SlotMode.OffGcd),
    };


    public Rotation Build(string settingFolder)
    {
        // 初始化设置
        ReaperSettings.Build(settingFolder);
        // 初始化QT （依赖了设置的数据）
        BuildQT();


        var rot = new Rotation(SlotResolvers)
        {
            TargetJob = Jobs.Reaper,
            AcrType = AcrType.HighEnd,
            MinLevel = 100,
            MaxLevel = 100,
            Description = "Reaper测试版",
        };

        // 添加各种事件回调
        rot.AddOpener(GetOpener);
        rot.SetRotationEventHandler(new ReaperRotationEventHandler());
        
        // 添加QT开关的时间轴行为
        rot.AddTriggerAction(new TriggerAction_QT());
        // rot.AddSlotSequences(new DoubleEnshroundSequence());

        return rot;
    }

    IOpener? GetOpener(uint level)
    {
        return null;
        // if (level < 100)
        //     return null;
        // return new Reaper_Opener100();
    }
    
    public IRotationUI GetRotationUI()
    {
        return QT;
    }

    // 构造函数里初始化QT
    public void BuildQT()
    {
        // JobViewSave是AE底层提供的QT设置存档类 在你自己的设置里定义即可
        // 第二个参数是你设置文件的Save类 第三个参数是QT窗口标题
        QT = new JobViewWindow(ReaperSettings.Instance.JobViewSave, ReaperSettings.Instance.Save, "LM Reaper[测试版]");
        QT.SetUpdateAction(OnUIUpdate); // 设置QT中的Update回调 不需要就不设置

        //添加QT分页 第一个参数是分页标题 第二个是分页里的内容
        QT.AddTab("通用", DrawQtGeneral);
        QT.AddTab("Dev", DrawQtDev);

        // 添加QT开关 第二个参数是默认值 (开or关) 第三个参数是鼠标悬浮时的tips
        QT.AddQt(QTKey.UsePotion, true);
        QT.AddQt(QTKey.Burst, true);
        QT.AddQt(QTKey.AOE, false);


        QT.AddQt(QTKey.Gluttony, true);
        QT.AddQt(QTKey.BloodStalk, true);
        QT.AddQt(QTKey.Enshroud, true);

        QT.AddQt(QTKey.ShadowofDeath, true);
        QT.AddQt(QTKey.poscheak, true);
        QT.AddQt(QTKey.UseTrueNorth, false);
        QT.AddQt(QTKey.Enshroud_first, false);

        // QT.AddQt(QTKey.ShadowofDeath, true);
        QT.AddQt(QTKey.PlentifulHarvest, true);
        QT.AddQt(QTKey.FinalBurst, false);




        // 添加快捷按钮 (带技能图标)
        /*QT.AddHotkey("战斗之声",
            new HotKeyResolver_NormalSpell(SpellsDefine.BattleVoice, SpellTargetType.Self));
        QT.AddHotkey("失血",
            new HotKeyResolver_NormalSpell(SpellsDefine.Bloodletter, SpellTargetType.Target));
        QT.AddHotkey("爆发药", new HotKeyResolver_Potion());
        QT.AddHotkey("极限技", new HotKeyResolver_LB());*/

        /*
        // 这是一个自定义的快捷按钮 一般用不到
        // 图片路径是相对路径 基于AEAssist(C|E)NVersion/AEAssist
        // 如果想用AE自带的图片资源 路径示例: Resources/AE2Logo.png
        QT.AddHotkey("极限技", new HotkeyResolver_General("#自定义图片路径", () =>
        {
            // 点击这个图片会触发什么行为
            LogHelper.Print("你好");
        }));
        */
    }

    // 设置界面
    public void OnDrawSetting()
    {
        ReaperSettingUI.Instance.Draw();
    }

    public void OnUIUpdate()
    {

    }

    public void DrawQtGeneral(JobViewWindow jobViewWindow)
    {   
        ImGui.Text("目前QT不可用");

        ImGui.Text("施工计划：");
        ImGui.Text("1.添加更多的技能");
        ImGui.Text("2.可选的动画锁定");
        ImGui.Text("3.完成QT的开发");
        ImGui.Text("4.添加更多的开场");
        ImGui.Text("5.更好的暴食释放时机");
        // ImGui.Text("GCD:" + GCDHelper.GetGCDCooldown() + "/" + GCDHelper.GetGCDDuration());
        // ImGui.Text("易伤:" + Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(AurasDefine.DeathsDesign, 100));
        // ImGui.Text("上一次中断:" + ReaperRotationEventHandler.times + "s");
        // ImGui.Text("大丰收RecentlyUsed:" + SpellsDefine.PlentifulHarvest.RecentlyUsed(1000));

    }

    public void DrawQtDev(JobViewWindow jobViewWindow)
    {
        ImGui.Text("画Dev信息");
        foreach (var v in jobViewWindow.GetQtArray())
        {
            ImGui.Text($"Qt按钮: {v}");
        }

        foreach (var v in jobViewWindow.GetHotkeyArray())
        {
            ImGui.Text($"Hotkey按钮: {v}");
        }
    }

    public void Dispose()
    {
        // 释放需要释放的东西 没有就留空
    }
}