using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public static string Scene = "";

    //private PageDrillIn PageDrillIn;

    /*private void Awake()
    {
        PageDrillIn = GetComponent<PageDrillIn>();
        GetComponent<CanvasGroup>().alpha = 0;
    }*/

    private void Start()
    {
        if (string.IsNullOrEmpty(Scene))
            return;
        StartAsync().Forget();
    }

    private async UniTaskVoid StartAsync()
    {
        //await PageDrillIn.StartAnimation().ToUniTask();

        //PageDrillIn.Easing = AnimationEasing.EaseOut;

        await UniTask.Yield();
        await UniTask.NextFrame();
        {
            var op = SceneManager.LoadSceneAsync(Scene);
            op.allowSceneActivation = false;

            Scene = "";

            while (!op.isDone)
            {
                if (op.progress >= 0.9f)
                {
                    //await PageDrillIn.StartAnimation().ToUniTask();
                    op.allowSceneActivation = true;
                }
                await UniTask.Yield();
                await UniTask.NextFrame();
            }
        }
    }
}
