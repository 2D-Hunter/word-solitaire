using UnityEngine;

public static class GameUtils
{
    public static bool IsFacebookBuild()
    {
        return FBPlayerData.instance != null && FBPlayerData.instance.BUILD_TYPE == "Facebook";
    }
    public static bool IsAndroidBuild()
    {
        return FBPlayerData.instance != null && FBPlayerData.instance.BUILD_TYPE == "Android";
    }
    public static bool IsIosBuild()
    {
        return FBPlayerData.instance != null && FBPlayerData.instance.BUILD_TYPE == "Ios";
    }

    public static int EffectiveCurrentLevel
    {
        get
        {
            return !InitManager.instance.isLevelRandomized
                ? FBPlayerData.instance.CURRENT_LEVEL - 1
                : InitManager.instance.nextRandomLevel;
        }
    }
}