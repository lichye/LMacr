namespace LM.Reaper;

// 存放战斗中的缓存数据 战斗重置后也会跟着清除
// 举例： 诗人需要记录上一次的双dot什么时候上的/吃了多少强化资源，来决定是否在恰当的时候立即刷新双dot
public class ReaperBattleData
{
    public static ReaperBattleData Instance = new();

    public ReaperBattleData()
    {
    
    }

    public int targetWithoutDeathsDesign = 0;

    public int targetNearbyCount = 0;

    public bool IsAbleDoubleEnshroud = false;

    //This means we are in the process of double enshroud
    public bool AutoDoubleEnshroud = false;

    public bool DoOneReaping = false;

    public int ShadowOfDeathInEnshroud = 0;

     

}