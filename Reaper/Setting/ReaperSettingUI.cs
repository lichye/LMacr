using AEAssist.GUI;
using ImGuiNET;

namespace LM.Reaper.Setting;

public class ReaperSettingUI
{
    public static ReaperSettingUI Instance = new();
    public ReaperSettings ReaperSettings => ReaperSettings.Instance;
    
    public void Draw()
    {
        ImGui.Checkbox("使用双附体", ref ReaperSettings.DoubleEnshroud);
        // ImGuiHelper.LeftInputInt("非爆发期Apex值达到多少时才使用", ref );
        
        if (ImGui.Button("Save"))
        {
            ReaperSettings.Instance.Save();
        }
    }
}