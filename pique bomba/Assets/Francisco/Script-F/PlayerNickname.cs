using Fusion;
using TMPro;
using UnityEngine;

public class PlayerNickname : NetworkBehaviour
{
    [Header("Texto 3D")]
    public TMP_Text nomeTexto;

    [Header("Referência do Display")]
    public Transform nameDisplay; // Arraste o NameDisplay aqui

    [Networked]
    public NetworkString<_16> NomeJogador { get; set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            string nome = PlayerPrefs.GetString("PlayerName", "Player");
            RPC_DefinirNome(nome);
        }

        AtualizarNome();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_DefinirNome(string nome)
    {
        NomeJogador = nome;
        AtualizarNome();
    }

    public override void Render()
    {
        AtualizarNome();
    }

    void AtualizarNome()
    {
        if (nomeTexto != null)
        {
            nomeTexto.text = NomeJogador.ToString();
        }
    }

    void LateUpdate()
    {
        if (Camera.main != null && nameDisplay != null)
        {
            // Faz o texto olhar para a câmera
            nameDisplay.forward = Camera.main.transform.forward;
        }
    }
}