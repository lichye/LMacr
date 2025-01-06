
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
        
        new (new oGCD_Lemure(),SlotMode.OffGcd),

        new (new oGCD_Sacrificium(),SlotMode.OffGcd),

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
            MinLevel = 100,
            MaxLevel = 100,
            Description = "人造剑尊杀戮-V0.7",
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
        // QT.AddTab("Dev", DrawQtDev);

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
        
        ImGui.Text("LM Reaper-V0.7");
        if (ImGui.Button("日随模式")) {
            ReaperSettings.Instance.Normal = true;
            ReaperSettings.Instance.HighEnd = false;
            ReaperSettings.Instance.ArcaneCircle_GCD = 2;
            ReaperSettings.Instance.BaseGCD_BehindFirst = true;
            ReaperSettings.Instance.PreHarpe = false;
            ReaperSettings.Instance.AutoEnshroud = true;
            ReaperSettings.Instance.Enshroud_threadhold = 50;
            ReaperSettings.Instance.ShadowofDeath_time = 5000;
            ReaperSettings.Instance.careAboutPos = false;
            ReaperSettings.Instance.DoubleEnshroud = false;
            ReaperSettings.Instance.StandardShroud = true;
        }
        ImGui.SameLine();
        if (ImGui.Button("高难模式")){
            ReaperSettings.Instance.Normal = false;
            ReaperSettings.Instance.HighEnd = true;
            ReaperSettings.Instance.ArcaneCircle_GCD = 2;
            ReaperSettings.Instance.BaseGCD_BehindFirst = true;
            ReaperSettings.Instance.PreHarpe = true;
            ReaperSettings.Instance.AutoEnshroud = true;
            ReaperSettings.Instance.Enshroud_threadhold = 60;
            ReaperSettings.Instance.ShadowofDeath_time = 3000;
            ReaperSettings.Instance.careAboutPos = true;
            ReaperSettings.Instance.DoubleEnshroud = true;
            ReaperSettings.Instance.StandardShroud = false;
        }

        
        if (ImGui.CollapsingHeader("起手设置")) {

            ImGui.Text("起手设置 2G/3G");
            ImGui.SliderInt("GCD", ref ReaperSettings.Instance.ArcaneCircle_GCD, 2, 3);
            ImGui.Checkbox("预读勾刃",ref ReaperSettings.Instance.PreHarpe);

            ImGui.Text("");
            ImGui.Text("爆发药ID:");
            ImGui.SameLine();
            ImGui.InputInt("整数输入", ref ReaperSettings.Instance.Gemdraught_id);
        }

        if(ImGui.CollapsingHeader("循环设置")){
            ImGui.Text("镰刀爆发期 -10 秒就开始了");
            ImGui.Text("如果资源正常，就会打双附体，资源不够就会打单附体");
            ImGui.Text("循环设置");
            if(ImGui.Checkbox("双附体循环",ref ReaperSettings.Instance.DoubleEnshroud)){
                ReaperSettings.Instance.StandardShroud = false;
            }
            ImGui.SameLine();
            if(ImGui.Checkbox("单附体循环",ref ReaperSettings.Instance.StandardShroud)){
                ReaperSettings.Instance.DoubleEnshroud = false;
            }
            // ImGui.Text("双附体触发时间");
            // ImGui.Text("时间太高或者太低都不行，请根据自己网速/动画锁调整");
            // ImGui.Text("完人打不进120就稍微拉高点");
            // ImGui.SliderInt("毫秒", ref ReaperSettings.Instance.preEnshroudTime, 5500,6500);
            // ImGui.InputInt("动画锁+网络延迟",ref ReaperSettings.Instance.AnimationLock);
        }

        if(ImGui.CollapsingHeader("身位buff设置")){
            ImGui.Text("死亡之影续buff时间");
            ImGui.SliderInt("毫秒", ref ReaperSettings.Instance.ShadowofDeath_time, 1000, 5000);

            ImGui.Text("");
            ImGui.Text("绞决-缢杀身位设置");
            ImGui.Checkbox("正确身位触发绞决-缢杀",ref ReaperSettings.Instance.careAboutPos);

            ImGui.Text("");
            ImGui.Checkbox("起手打背",ref ReaperSettings.Instance.BaseGCD_BehindFirst);

            ImGui.Text("");
            ImGui.Text("资源期附体设置");
            ImGui.Checkbox("自动附体",ref ReaperSettings.Instance.AutoEnshroud);

            ImGui.Text("资源期自动附体多少蓝量触发");
            ImGui.SliderInt("蓝量", ref ReaperSettings.Instance.Enshroud_threadhold, 50, 100);
        }
        

    }

    public void DrawQtDev(JobViewWindow jobViewWindow)
    {
        ImGui.Text("画Dev信息");
        // foreach (var v in jobViewWindow.GetQtArray())
        // {
        //     ImGui.Text($"Qt按钮: {v}");
        // }

        // foreach (var v in jobViewWindow.GetHotkeyArray())
        // {
        //     ImGui.Text($"Hotkey按钮: {v}");
        // }
        ImGui.Text($"IsAbleDoubleENshroud:{ReaperBattleData.Instance.IsAbleDoubleEnshroud}");
        ImGui.Text($"AnimationLock:{ReaperSettings.Instance.AnimationLock}");
        ImGui.Text($"GCD_Time:{ReaperSettings.Instance.GCD_Time}");
        ImGui.Text($"ShadowofDeath_time:{ReaperSettings.Instance.ShadowofDeath_time}");
        ImGui.Text($"preEnshroudTime:{ReaperSettings.Instance.preEnshroudTime}");
    }

    public void Dispose()
    {
    }
}