using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;


public static class AssetBundleCreateRequestExtensions
{

    public static TaskAwaiter<AssetBundle> GetAwaiter(this AssetBundleCreateRequest request)
    {
        var tcs = new TaskCompletionSource<AssetBundle>();
        request.completed += (_) => tcs.SetResult(request.assetBundle);
        return tcs.Task.GetAwaiter();
    }

}

public static class FloatExtensions
{

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

}
