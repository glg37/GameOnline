using Fusion;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public NetworkPrefabRef playerPrefab;

    public override void Spawned()
    {
            Runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, Runner.LocalPlayer);
    }
}