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
}