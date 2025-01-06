using System;
using System.IO;
using AEAssist;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Helper;
using AEAssist.IO;

namespace LM.Reaper.Setting;

/// <summary>
/// 配置文件适合放一些一般不会在战斗中随时调整的开关数据
/// 如果一些开关需要在战斗中调整 或者提供给时间轴操作 那就用QT
/// 非开关类型的配置都放配置里 比如诗人绝峰能量配置
/// </summary>
public class ReaperSettings
{
    public static ReaperSettings Instance;

    #region 标准模板代码 可以直接复制后改掉类名即可
    private static string path;
    public static void Build(string settingPath)
    { 
        
        path = Path.Combine(settingPath, nameof(ReaperSettings) + ".json");
        if (!File.Exists(path))
        {
            Instance = new ReaperSettings();
            Instance.Save();
            return;
        }
        try
        {
            Instance = JsonHelper.FromJson<ReaperSettings>(File.ReadAllText(path));
        }
        catch (Exception e)
        {
            Instance = new();
            LogHelper.Error(e.ToString());
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, JsonHelper.ToJson(this));
    }
    #endregion

    public bool HighEnd = false;

    public bool Normal = true;

    public bool PreHarpe = true;
    public bool OpenerNoPos = true;

    public bool careAboutPos = true;

    public bool BaseGCD_BehindFirst = true;

    public int AnimationLock = 800;

    public bool DoubleEnshroud = true;

    public int preEnshroudTime = 6500;

    public bool StandardShroud = false;

    public bool AutoEnshroud = true;

    public int Enshroud_threadhold = 50;

    public int Harpe_time = 1500;

    public int ShadowofDeath_time = 3000;

    public int GCD_Time = 2500;

    public int ArcaneCircle_GCD = 2;
    
    public JobViewSave JobViewSave = new(); // QT设置存档

    public int Gemdraught_id = 0;
}