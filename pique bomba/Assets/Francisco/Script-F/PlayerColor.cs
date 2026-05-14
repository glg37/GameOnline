using Fusion;
using UnityEngine;

public class PlayerColor : NetworkBehaviour
{
    [Networked]
    public int CorIndex { get; set; }

    public Renderer playerRenderer;

    public static readonly Color[] cores =
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow
    };

    public override void Spawned()
    {
        AtualizarCor();
    }

    public override void Render()
    {
        AtualizarCor();
    }

    void AtualizarCor()
    {
        if (playerRenderer == null)
            return;

        if (CorIndex < 0 || CorIndex >= cores.Length)
            return;

        playerRenderer.material.color = cores[CorIndex];
    }

    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_SetColor(int index)
    {
        CorIndex = index;
    }
}