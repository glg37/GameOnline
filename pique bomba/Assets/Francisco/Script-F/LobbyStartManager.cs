using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LobbyStartManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject painelLobby;
    public TMP_Text jogadoresTexto;
    public TMP_Text timerTexto;
    public Button botaoComecar;

    [Header("Cores")]
    public Button[] botoesCores;

    [Header("Config")]
    public int maxJogadores = 1;
    public float tempoLiberarBotao = 2f;

    private float tempoAtual;
    private PlayerColor meuPlayer;
    private NetworkRunner runner;

    public static bool LobbyAtivo;

    private TickTimer startTimer;
    private bool jogoComecou;

    void Start()
    {
        runner = Object.FindFirstObjectByType<NetworkRunner>();

        painelLobby.SetActive(true);
        botaoComecar.gameObject.SetActive(false);
        timerTexto.gameObject.SetActive(false);

        tempoAtual = tempoLiberarBotao;

        LobbyAtivo = true;
    }

    void Update()
    {
        if (runner == null)
            return;

        AtualizarJogadores();
        ProcurarPlayerLocal();
        AtualizarBotoesCores();

        if (jogoComecou)
            return;

        // host libera botão
        if (runner.IsServer)
        {
            tempoAtual -= Time.deltaTime;

            if (tempoAtual <= 0 && runner.ActivePlayers.Count() >= 1)
            {
                botaoComecar.gameObject.SetActive(true);
            }
        }

        if (!runner.IsServer)
        {
            botaoComecar.gameObject.SetActive(false);
        }

        // countdown
        if (startTimer.IsRunning)
        {
            int t = Mathf.CeilToInt(startTimer.RemainingTime(runner) ?? 0);

            timerTexto.gameObject.SetActive(true);
            timerTexto.text = t.ToString();

            if (t <= 0)
            {
                jogoComecou = true;
                LobbyAtivo = false;
                painelLobby.SetActive(false);
            }
        }
    }

    void AtualizarJogadores()
    {
        if (runner == null || jogadoresTexto == null)
            return;

        int count = runner.ActivePlayers.Count();

        jogadoresTexto.text = count + "/" + maxJogadores;
    }

    void ProcurarPlayerLocal()
    {
        if (meuPlayer != null || runner == null)
            return;

        foreach (var p in runner.ActivePlayers)
        {
            var obj = runner.GetPlayerObject(p);

            if (obj == null)
                continue;

            if (!obj.HasInputAuthority)
                continue;

            meuPlayer = obj.GetComponent<PlayerColor>();
            break;
        }
    }

    bool CorLivre(int index)
    {
        foreach (var p in runner.ActivePlayers)
        {
            var obj = runner.GetPlayerObject(p);

            if (obj == null)
                continue;

            var pc = obj.GetComponent<PlayerColor>();

            if (pc != null && pc.CorIndex == index)
                return false;
        }
        return true;
    }

    void AtualizarBotoesCores()
    {
        if (botoesCores == null)
            return;

        for (int i = 0; i < botoesCores.Length; i++)
        {
            if (botoesCores[i] == null)
                continue;

            botoesCores[i].interactable = CorLivre(i);
        }
    }

    // BOTÃO COR
    public void EscolherCor(int index)
    {
        if (meuPlayer == null)
            return;

        if (!CorLivre(index))
            return;

        meuPlayer.RPC_EscolherCor(index);
    }

    // BOTÃO COMEÇAR
    public void ClicarComecar()
    {
        if (!runner.IsServer)
            return;

        RPC_Iniciar();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_Iniciar()
    {
        startTimer = TickTimer.CreateFromSeconds(runner, 3f);
    }
}