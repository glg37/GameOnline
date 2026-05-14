using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

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
    public int maxJogadores = 2;
    public float tempoLiberarBotao = 2f;
    public int minJogadoresParaStart = 1;

    private NetworkRunner runner;
    private PlayerColor meuPlayer;

    private float tempoAtual;
    private bool jogoComecou;

    private TickTimer startTimer;

    public static bool LobbyAtivo;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        runner = Object.FindAnyObjectByType<NetworkRunner>();

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

        ControleBotaoComecar();

        if (jogoComecou)
            return;

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

    // -------------------------
    // PLAYERS
    // -------------------------
    void AtualizarJogadores()
    {
        if (runner == null || jogadoresTexto == null)
            return;

        int count = runner.ActivePlayers != null
            ? runner.ActivePlayers.Count()
            : 0;

        jogadoresTexto.text = count + "/" + maxJogadores;
    }

    // -------------------------
    // PLAYER LOCAL
    // -------------------------
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

    // -------------------------
    // CORES
    // -------------------------
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

    public void EscolherCor(int index)
    {
        if (meuPlayer == null)
            return;

        meuPlayer.RPC_SetColor(index);
    }

    // -------------------------
    // BOTÃO START (FIX REAL)
    // -------------------------
    void ControleBotaoComecar()
    {
        if (runner == null || botaoComecar == null)
            return;

        bool isHost = runner.IsSharedModeMasterClient;

        if (!isHost)
        {
            botaoComecar.gameObject.SetActive(false);
            return;
        }

        tempoAtual -= Time.deltaTime;

        bool podeMostrar =
            tempoAtual <= 0 &&
            runner.ActivePlayers.Count() >= minJogadoresParaStart;

        botaoComecar.gameObject.SetActive(podeMostrar);
    }

    public void ClicarComecar()
    {
        if (runner == null)
            return;

        if (!runner.IsSharedModeMasterClient)
            return;

        RPC_Iniciar();
    }

    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_Iniciar()
    {
        startTimer = TickTimer.CreateFromSeconds(runner, 3f);
    }
}