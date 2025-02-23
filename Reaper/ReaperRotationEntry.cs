
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
using System.Numerics; 
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

        new(new GCD_Gibbet(),SlotMode.Gcd),

        new(new GCD_SoulSlice(),SlotMode.Gcd),

        new(new GCD_HarvestMoon(),SlotMode.Gcd),

        new(new GCD_AOE(),SlotMode.Gcd),

        new(new GCD_Base(),SlotMode.Gcd),
        
        // ogcd lists
        
        // the order be like：

        // Enshroud
        
        // Gemdraught 

        // Sacrificium
        
        // Lemure

        // Gluttony

        // BloodStalk
        
        new (new oGCD_Gemdraught(),SlotMode.OffGcd),
        
        new (new oGCD_Enshroud(),SlotMode.OffGcd),

        new (new oGCD_ArcaneCircle(),SlotMode.OffGcd),
        
        new (new oGCD_Lemure(),SlotMode.OffGcd),

        new (new oGCD_Sacrificium(),SlotMode.OffGcd),

        new (new oGCD_TrueNorth(),SlotMode.OffGcd),

        new (new oGCD_Gluttony(),SlotMode.OffGcd),

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
            Description = "LM镰刀 \n支持全等级亲信/日随/高难",
        };

        rot.AddOpener(GetOpener);
        rot.SetRotationEventHandler(new ReaperRotationEventHandler());
        rot.AddTriggerAction(new TriggerAction_QT());
        return rot;
    }

    IOpener? GetOpener(uint level)
    {   
        if(Core.Me.Level>=96)
            return new Reaper_Opener_100();
        
        if(Core.Me.Level>=76)
            return new Reaper_Opener_80();

        return new Reaper_Opener_70();
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
        QT.AddTab("模式设置", DrawQtGeneral);
        QT.AddTab("日随设置", DrawNormalSetting);
        QT.AddTab("高难设置", DrawHighEndSetting);
        QT.AddTab("更新日志", DrawUpdateTimeline);
        // QT.AddTab("Dev", DrawQtDev);

        // 添加QT开关 第二个参数是默认值 (开or关) 第三个参数是鼠标悬浮时的tips
        
        QT.AddQt(QTKey.Burst, true);
        QT.AddQt(QTKey.AOE, true);
        QT.AddQt(QTKey.HarvestMoon, true);
        QT.AddQt(QTKey.Enshroud, true);
        QT.AddQt(QTKey.EnableGemdraught, true);
        QT.AddQt(QTKey.AutoTrueNorth, true);
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
        if (ReaperSettings.Instance.Mate){
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.1f, 0.1f, 1f, 1.0f)); // 选中时为黄色
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.8f, 0.0f, 0.0f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.6f, 0.0f, 0.0f, 1.0f));
        }
        else{
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.5f, 0.5f, 0.5f, 1.0f)); // 未选中时为灰色
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.7f, 0.7f, 0.7f, 1.0f));
        }
        if (ImGui.Button("亲信模式")) {
            ReaperSettings.Instance.Normal = false;
            ReaperSettings.Instance.HighEnd = false;
            ReaperSettings.Instance.Mate = true;
            ReaperSettings.Instance.ArcaneCircle_GCD = 2;
            ReaperSettings.Instance.BaseGCD_BehindFirst = true;
            ReaperSettings.Instance.PreHarpe = false;
            ReaperSettings.Instance.AutoEnshroud = true;
            ReaperSettings.Instance.Enshroud_threadhold = 50;
            ReaperSettings.Instance.ShadowofDeath_time = 5000;
            ReaperSettings.Instance.careAboutPos = false;
            ReaperSettings.Instance.DoubleEnshroud = false;
            ReaperSettings.Instance.StandardShroud = true;
            ReaperRotationEntry.QT.SetQt(QTKey.EnableGemdraught, false);
        }
        ImGui.SameLine();
        if (ReaperSettings.Instance.Normal)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.1f, 1.0f, 0.1f, 1.0f)); // 选中时为绿色
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.8f, 0.0f, 0.0f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.6f, 0.0f, 0.0f, 1.0f));
        }
        else
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.5f, 0.5f, 0.5f, 1.0f)); // 未选中时为灰色
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.7f, 0.7f, 0.7f, 1.0f));
        }
        if (ImGui.Button("日随模式")) {
            ReaperSettings.Instance.Normal = true;
            ReaperSettings.Instance.HighEnd = false;
            ReaperSettings.Instance.Mate = false;
            ReaperSettings.Instance.ArcaneCircle_GCD = 2;
            ReaperSettings.Instance.BaseGCD_BehindFirst = true;
            ReaperSettings.Instance.PreHarpe = false;
            ReaperSettings.Instance.AutoEnshroud = true;
            ReaperSettings.Instance.Enshroud_threadhold = 50;
            ReaperSettings.Instance.ShadowofDeath_time = 5000;
            ReaperSettings.Instance.careAboutPos = false;
            ReaperSettings.Instance.DoubleEnshroud = false;
            ReaperSettings.Instance.StandardShroud = true;
            ReaperRotationEntry.QT.SetQt(QTKey.EnableGemdraught, false);
        }
        ImGui.SameLine();
        if (ReaperSettings.Instance.HighEnd)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1.0f, 0.0f, 0.0f, 1.0f)); // 选中时为红色
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.8f, 0.0f, 0.0f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.6f, 0.0f, 0.0f, 1.0f));
        }
        else
        {
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.5f, 0.5f, 0.5f, 1.0f)); // 未选中时为灰色
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.7f, 0.7f, 0.7f, 1.0f));
        }
        if (ImGui.Button("高难模式")){
            ReaperSettings.Instance.Normal = false;
            ReaperSettings.Instance.HighEnd = true;
            ReaperSettings.Instance.Mate = false;
            ReaperSettings.Instance.ArcaneCircle_GCD = 2;
            ReaperSettings.Instance.BaseGCD_BehindFirst = true;
            ReaperSettings.Instance.PreHarpe = true;
            ReaperSettings.Instance.AutoEnshroud = true;
            ReaperSettings.Instance.Enshroud_threadhold = 60;
            ReaperSettings.Instance.ShadowofDeath_time = 3500;
            ReaperSettings.Instance.careAboutPos = true;
            ReaperSettings.Instance.DoubleEnshroud = true;
            ReaperSettings.Instance.StandardShroud = false;
            ReaperRotationEntry.QT.SetQt(QTKey.EnableGemdraught, true);
        }

        ImGui.Text("施工计划：");
        ImGui.Text("身位指示器");
        ImGui.Text("2-8爆发药支持");
        ImGui.Text("日随更加合理的爆发使用");
    }

    public void DrawNormalSetting(JobViewWindow jobViewWindow)
    {
        ImGui.Text("日随模式");
        ImGui.Text("使用暴食对齐120工整循环");

        ImGui.Text("死亡之影续buff时间");
        ImGui.SliderInt("毫秒", ref ReaperSettings.Instance.ShadowofDeath_time, 1000, 5000);
        ImGui.Checkbox("身位正确则使用身位技能，否则憋着",ref ReaperSettings.Instance.careAboutPos);
        ImGui.Checkbox("起手打背，不勾选打侧",ref ReaperSettings.Instance.BaseGCD_BehindFirst);
    }

    public void DrawHighEndSetting(JobViewWindow jobViewWindow)
    {   
        ImGui.Text("高难模式下，使用双附体");
        ImGui.Text("死亡之影续buff时间");
        ImGui.SliderInt("毫秒", ref ReaperSettings.Instance.ShadowofDeath_time, 1000, 5000);

        if (ImGui.CollapsingHeader("起手设置")) {
            ImGui.Text("起手设置 2G/3G");
            ImGui.SameLine();
            ImGui.SliderInt("GCD", ref ReaperSettings.Instance.ArcaneCircle_GCD, 2, 3);
            ImGui.Checkbox("预读勾刃",ref ReaperSettings.Instance.PreHarpe);
        }

        if(ImGui.CollapsingHeader("身位设置")){
            ImGui.Checkbox("身位正确则使用身位技能，否则憋着",ref ReaperSettings.Instance.careAboutPos);
            ImGui.Checkbox("起手打背，不勾选打侧",ref ReaperSettings.Instance.BaseGCD_BehindFirst);
            
        }

        if(ImGui.CollapsingHeader("附体设置")){
            // DrawEnshroudSetting();
            if (ReaperSettings.Instance.DoubleEnshroud)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1.0f, 0.0f, 0.0f, 1.0f)); // 选中时为红色
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.8f, 0.0f, 0.0f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.6f, 0.0f, 0.0f, 1.0f));
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.5f, 0.5f, 0.5f, 1.0f)); // 未选中时为灰色
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.7f, 0.7f, 0.7f, 1.0f));
            }

            if (ImGui.Button("双附体")) {
                ReaperSettings.Instance.DoubleEnshroud = true;
                ReaperSettings.Instance.StandardShroud = false;
            }

            if (ReaperSettings.Instance.StandardShroud)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1.0f, 0.0f, 0.0f, 1.0f)); // 选中时为红色
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.8f, 0.0f, 0.0f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.6f, 0.0f, 0.0f, 1.0f));
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.5f, 0.5f, 0.5f, 1.0f)); // 未选中时为灰色
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.6f, 0.6f, 0.6f, 1.0f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.7f, 0.7f, 0.7f, 1.0f));
            }
            ImGui.SameLine();
            if(ImGui.Button("标准附体")){
                ReaperSettings.Instance.DoubleEnshroud = false;
                ReaperSettings.Instance.StandardShroud = true;
            }
        
            ImGui.Checkbox("自动附体",ref ReaperSettings.Instance.AutoEnshroud);
            ImGui.Text("资源期自动附体多少蓝量触发");
            ImGui.SliderInt("蓝量", ref ReaperSettings.Instance.Enshroud_threadhold, 50, 100);
        }
    }
    public void DrawQtDev(JobViewWindow jobViewWindow)
    {
        ImGui.Text("画Dev信息");
        ImGui.Text($"DoubleEnshroud:{ReaperSettings.Instance.DoubleEnshroud}");
        ImGui.Text($"targetWithoutDeathsDesign:{ReaperBattleData.Instance.targetWithoutDeathsDesign}");
        ImGui.Text($"targetNearbyCount:{ReaperBattleData.Instance.targetNearbyCount}");
    }

    public void DrawUpdateTimeline(JobViewWindow jobViewWindow){
        ImGui.Text("更新日志");
        ImGui.Text("Feb 23, 2025");
        ImGui.Text("更新了自动真北逻辑");
        ImGui.Text("更新了亲信模式");
        ImGui.Text("优化了团契移动时的替代技能");
        ImGui.Text("\n");
        ImGui.Text("Feb 16, 2025");
        ImGui.Text("修复了爆发药造成的bug");
        ImGui.Text("\n");
        ImGui.Text("Feb 15, 2025");
        ImGui.Text("更新了爆发药逻辑");
        ImGui.Text("优化了起手逻辑");
        ImGui.Text("\n");
        ImGui.Text("Dec 29, 2024");
        ImGui.Text("更新了开场爆发和双附体");
        ImGui.Text("\n");
        ImGui.Text("Dec 25, 2024");
        ImGui.Text("完成了主体内容");
    }
    public void Dispose()
    {
    }
}