using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class SimpleMultiplayer : MonoBehaviour
{
    public NetworkRunner runnerPrefab;

    private static NetworkRunner runnerInstance;

    async void Awake()
    {
        if (runnerInstance != null)
        {
            return;
        }

        runnerInstance = Instantiate(runnerPrefab);

        runnerInstance.ProvideInput = true;

        DontDestroyOnLoad(runnerInstance.gameObject);

        await runnerInstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "MinhaSala",
            Scene = SceneRef.FromIndex(1),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }
}