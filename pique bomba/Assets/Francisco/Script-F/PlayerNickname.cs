using Fusion;
using TMPro;
using UnityEngine;

public class PlayerNickname : NetworkBehaviour
{
    [Header("Texto 3D")]
    public TMP_Text nomeTexto;

    [Networked]
    public NetworkString<_16> NomeJogador { get; set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            NomeJogador = PlayerPrefs.GetString("PlayerName", "Player");
        }

        AtualizarNome();
    }

    public override void Render()
    {
        AtualizarNome();
    }

    void AtualizarNome()
    {
        nomeTexto.text = NomeJogador.ToString();
    }

    void LateUpdate()
    {
        if (Camera.main != null)
        {
            // Faz o texto olhar para a câmera
            nomeTexto.transform.forward = Camera.main.transform.forward;
        }
    }
}