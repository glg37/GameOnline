using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class MultiplayerMenu : MonoBehaviour
{
    public NetworkRunner runnerPrefab;

    private NetworkRunner runner;

    public async void StartGame()
    {
        runner = Instantiate(runnerPrefab);

        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "Sala1",
            Scene = SceneRef.FromIndex(1),
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        });
    }
}
