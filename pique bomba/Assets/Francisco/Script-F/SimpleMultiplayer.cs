using Fusion;
using UnityEngine;
using System.Threading.Tasks;

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
            Scene = SceneRef.FromIndex(0),
            SceneManager = GetComponent<NetworkSceneManagerDefault>()
        });
    }
}