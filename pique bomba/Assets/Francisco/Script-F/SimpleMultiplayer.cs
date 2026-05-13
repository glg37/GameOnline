using Fusion;
using UnityEngine;

public class SimpleMultiplayer : MonoBehaviour
{
    public NetworkRunner runnerPrefab;

    private NetworkRunner runner;

    async void Start()
    {
        NetworkRunner existingRunner =
            FindAnyObjectByType<NetworkRunner>();

        if (existingRunner != null)
        {
            Destroy(existingRunner.gameObject);
        }

        runner = Instantiate(runnerPrefab);

        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = "MinhaSala",
            Scene = SceneRef.None,
            SceneManager =
                runner.GetComponent<NetworkSceneManagerDefault>()
        });
    }
}