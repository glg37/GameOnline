using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public NetworkPrefabRef playerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("Player entrou!");

        if (player == Runner.LocalPlayer)
        {
            Debug.Log("Spawnando player local!");

            Runner.Spawn(
                playerPrefab,
                new Vector3(0, 3, 0),
                Quaternion.identity,
                player
            );
        }
    }
}