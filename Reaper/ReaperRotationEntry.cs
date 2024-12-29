
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
        
        // PentifulHarvest

        // Perfectio

        // Gibbet + Gallows
        
        // single_target_base
        
        // aoe_base

        new(new GCD_ShadowofDeath(),SlotMode.Gcd),

        new(new GCD_PlentifulHarvest(),SlotMode.Gcd),

        new(new GCD_Perfectio(),SlotMode.Gcd),

        new(new GCD_Reaping(),SlotMode.Gcd),

        new(new GCD_HarvestMoon(),SlotMode.Gcd),

        new(new GCD_Gibbet(),SlotMode.Gcd),

        new(new GCD_SoulSlice(),SlotMode.Gcd),

        new(new GCD_AOE(),SlotMode.Gcd),

        new(new GCD_Base(),SlotMode.Gcd),
        
        // ogcd lists
        
        // the order be like：

        // Enshroud
        
        // Gemdraught 
        // slot.Add(Spell.CreatePotion());

        // Sacrificium
        
        // Lemure

        // Gluttony

        // BloodStalk
        
        
        new (new oGCD_Enshroud(),SlotMode.OffGcd),

        new (new oGCD_ArcaneCircle(),SlotMode.OffGcd),
        
        new (new oGCD_Sacrificium(),SlotMode.OffGcd),

        new (new oGCD_Lemure(),SlotMode.OffGcd),

        new (new offGCD_Gluttony(),SlotMode.OffGcd),

        new (new oGCD_BloodStalk(),SlotMode.OffGcd),
    };


    public Rotation Build(string settingFolder)
    {
        ReaperSettings.Build(settingFolder);
        BuildQT();
        var rot = new Rotation(SlotResolvers)
        {
            TargetJob = Jobs.Reaper,
            AcrType = AcrType.Both,
            MinLevel = 1,
            MaxLevel = 100,
            Description = "Reaper测试版",
        };

        rot.AddOpener(GetOpener);
        rot.SetRotationEventHandler(new ReaperRotationEventHandler());
        rot.AddTriggerAction(new TriggerAction_QT());
        return rot;
    }

    IOpener? GetOpener(uint level)
    {
        if(level == 100)
        {
            return new Reaper_Opener100();
        }
        return null;
    }
    
    public IRotationUI GetRotationUI()
    {
        return QT;
    }

    public void BuildQT()
    {
        QT = new JobViewWindow(ReaperSettings.Instance.JobViewSave, ReaperSettings.Instance.Save, "LM Reaper-V0.5");
        QT.SetUpdateAction(OnUIUpdate); // 设置QT中的Update回调 不需要就不设置
        //添加QT分页 第一个参数是分页标题 第二个是分页里的内容
        QT.AddTab("必看设置", DrawQtGeneral);
        QT.AddTab("Dev", DrawQtDev);

        // 添加QT开关 第二个参数是默认值 (开or关) 第三个参数是鼠标悬浮时的tips
        // QT.AddQt(QTKey.UsePotion, true);
        // QT.AddQt(QTKey.Burst, true);
        
        QT.AddQt(QTKey.AOE, true);
        QT.AddQt(QTKey.HarvestMoon, true);
        QT.AddQt(QTKey.Enshroud, true);
        // QT.GetSave.DoubleEnshroud;

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
        
        ImGui.Text("LM Reaper-V0.5");

        ImGui.Text("作者: Logica Magna");

        ImGui.Text("玩好镰刀的关键在怎么开附体!");
        
        if (ImGui.CollapsingHeader("基础设置")) {
            ImGui.Text("神秘环GCD设置");
            ImGui.SliderInt("GCD", ref ReaperSettings.Instance.ArcaneCircle_GCD, 2, 3);

            ImGui.Text("附体设置");

            ImGui.Checkbox("爆发期双附体",ref ReaperSettings.Instance.DoubleEnshroud);

            ImGui.Checkbox("资源期自动附体",ref ReaperSettings.Instance.AutoEnshroud);

            ImGui.Text("资源期自动附体多少蓝量触发");
            ImGui.SliderInt("蓝量", ref ReaperSettings.Instance.Enshroud_threadhold, 50, 100);

            ImGui.Text("死亡之影续buff时间");
            ImGui.SliderInt("毫秒", ref ReaperSettings.Instance.ShadowofDeath_time, 1000, 5000);

            ImGui.Text("爆发药ID:");
            ImGui.SameLine();
            ImGui.InputInt("整数输入", ref ReaperSettings.Instance.Gemdraught_id);

            // ImGui.Text("起手设置");
            // ImGui::Text("起手设置");
            // static const char* openers[] = { "三阴起手", "三阳起手", "五气朝元" };
            // static int current_opener = 0;
            // ImGui::Combo("起手选择", &current_opener, openers, IM_ARRAYSIZE(openers));
            // ImGui::Text("选择起手方式：%s", openers[current_opener]);
            // 爆发药设置
            // ImGui::Text("爆发药设置");
            // static const char* potion_times[] = { "06 (默认)", "12", "18", "24" };
            // static int current_potion_time = 0;
            // ImGui::Combo("选择爆发药时间", &current_potion_time, potion_times, IM_ARRAYSIZE(potion_times));

            // // 倒数设置部分
            // ImGui::Text("倒数设置");
            // static bool countdown_14 = false, countdown_3 = true, countdown_05 = true;
            // ImGui::Checkbox("倒数14s真言", &countdown_14);
            // ImGui::SameLine();
            // ImGui::Checkbox("倒数3s真北", &countdown_3);
            // ImGui::SameLine();
            // ImGui::Checkbox("倒数0.5s突进", &countdown_05);

            // // 通用设置部分
            // ImGui::Text("通用设置");
            // static bool aoe_targeting = false, auto_true_north = true, keep_true_north = true;
            // ImGui::Checkbox("AOE智能目标", &aoe_targeting);
            // ImGui::Checkbox("自动真北 (检测勾选BUFF)", &auto_true_north);
            // ImGui::Checkbox("留一层真北 (如果你开启了自动真北)", &keep_true_north);

        }

        // if (ImGui.CollapsingHeader("轴控设置(高难用)")) {
        //     ImGui.Text("这里是轴控设置的内容");
        //     // 添加更多的设置控件...
        // }

        // if (ImGui.CollapsingHeader("日随设置")) {
        //     ImGui.Text("这里是日随设置的内容");
        //     // 添加更多的设置控件...
        // }

        // if (ImGui.CollapsingHeader("自回设置")) {
        //     ImGui.Text("这里是自回设置的内容");
        //     // 添加更多的设置控件...
        // }

        // ImGui.Checkbox("双附体",ref ReaperSettings.Instance.DoubleEnshroud);

        // if (ImGui.TreeNode("Options")) {
        //     ImGui.Text("Option 1");
        //     ImGui.Text("Option 2");
        //     ImGui.TreePop();
        // }
        // ImGui.Checkbox("双附体",ref QT.GetQ);
        // ImGui.Checkbox("双附体",ref RB.DoubleEnshroud);
        // ImGui.Text("施工计划：");
        // ImGui.Text("1.添加更多的技能");
        // ImGui.Text("2.可选的动画锁定");
        // ImGui.Text("3.完成QT的开发");
        // ImGui.Text("4.添加更多的开场");
        // ImGui.Text("5.更好的暴食释放时机");
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
        ImGui.Text($"ShadowofDeath_time:{ReaperSettings.Instance.ShadowofDeath_time}");
    }

    public void Dispose()
    {
    }
}