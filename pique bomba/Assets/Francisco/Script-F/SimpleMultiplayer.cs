using UnityEngine;
using Fusion;

public class SimpleMultiplayer : MonoBehaviour
{
    public NetworkRunner runnerPrefab;

    async void Start()
    {
        // destrói runners antigos
        foreach (NetworkRunner oldRunner in FindObjectsByType<NetworkRunner>(FindObjectsSortMode.None))
        {
            Destroy(oldRunner.gameObject);
        }

        // cria novo runner
        NetworkRunner runner = Instantiate(runnerPrefab);

        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "MinhaSala"
        });
    }
}