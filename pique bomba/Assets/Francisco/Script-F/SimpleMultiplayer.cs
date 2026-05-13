using Fusion;
using UnityEngine;

public class SimpleMultiplayer : MonoBehaviour
{
    private NetworkRunner runner;

    async void Start()
    {
        runner = GetComponent<NetworkRunner>();

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "MinhaSala",
            Scene = SceneRef.None,
            SceneManager = GetComponent<NetworkSceneManagerDefault>()
        });
    }
}
