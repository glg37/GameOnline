using Fusion;
using TMPro;
using UnityEngine;

public class PlayerNickname : NetworkBehaviour
{
    public TMP_Text nomeTexto;

    [Networked]
    public string NomeJogador { get; set; }

    public override void Spawned()
    {
    
        if (Object.HasInputAuthority)
        {
            NomeJogador = PlayerPrefs.GetString("PlayerName", "Player");

            
            nomeTexto.gameObject.SetActive(false);
        }

        AtualizarNome();
    }

    public override void Render()
    {
        AtualizarNome();
    }

    void AtualizarNome()
    {
        nomeTexto.text = NomeJogador;
    }

    void LateUpdate()
    {
        if (Camera.main != null)
        {
            nomeTexto.transform.LookAt(Camera.main.transform);
        }
    }
}