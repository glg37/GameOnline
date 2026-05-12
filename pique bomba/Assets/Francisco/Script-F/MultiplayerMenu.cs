using Fusion;
using TMPro;
using UnityEngine;

public class MultiplayerMenu : MonoBehaviour
{
    [Header("Fusion")]
    public NetworkRunner runnerPrefab;

    private NetworkRunner runner;

    [Header("Nome do Jogador")]
    public TMP_InputField inputNome;

    [Header("Criar Sala")]
    public TMP_InputField inputCriarSala;

    [Header("Entrar na Sala")]
    public TMP_InputField inputEntrarSala;

    [Header("Painéis")]
    public GameObject painelSala;
    public GameObject painelCreditos;

    [Header("Animators")]
    public Animator animatorSala;
    public Animator animatorCreditos;

    void Start()
    {
        // Painéis começam fechados
        if (painelSala != null)
        {
            painelSala.SetActive(false);
        }

        if (painelCreditos != null)
        {
            painelCreditos.SetActive(false);
        }
    }

    // =====================================================
    // ABRIR PAINEL SALA
    // =====================================================

    public void AbrirPainelSala()
    {
        painelSala.SetActive(true);

        animatorSala.SetTrigger("Abrir");
    }

    // =====================================================
    // FECHAR PAINEL SALA
    // =====================================================

    public void FecharPainelSala()
    {
        animatorSala.SetTrigger("Fechar");

        Invoke(nameof(DesativarPainelSala), 1f);
    }

    void DesativarPainelSala()
    {
        painelSala.SetActive(false);
    }

    // =====================================================
    // ABRIR CRÉDITOS
    // =====================================================

    public void AbrirCreditos()
    {
        painelCreditos.SetActive(true);

        animatorCreditos.SetTrigger("Abrir");
    }

    // =====================================================
    // FECHAR CRÉDITOS
    // =====================================================

    public void FecharCreditos()
    {
        animatorCreditos.SetTrigger("Fechar");

        Invoke(nameof(DesativarCreditos), 1f);
    }

    void DesativarCreditos()
    {
        painelCreditos.SetActive(false);
    }

    // =====================================================
    // CRIAR SALA
    // =====================================================

    public async void CriarSala()
    {
        // Nome jogador
        string nomeJogador = inputNome.text;

        if (string.IsNullOrWhiteSpace(nomeJogador))
        {
            nomeJogador = "Player";
        }

        // Nome sala
        string nomeSala = inputCriarSala.text;

        if (string.IsNullOrWhiteSpace(nomeSala))
        {
            Debug.Log("Digite um código para criar a sala!");
            return;
        }

        // Salvar nome
        PlayerPrefs.SetString("PlayerName", nomeJogador);

        // Criar runner
        runner = Instantiate(runnerPrefab);

        runner.ProvideInput = true;

        // Criar sala
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,

            SessionName = nomeSala,

            Scene = SceneRef.FromIndex(1),

            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        });

        if (!result.Ok)
        {
            Debug.Log("Erro ao criar sala!");
        }
    }

    // =====================================================
    // ENTRAR NA SALA
    // =====================================================

    public async void EntrarSala()
    {
        // Nome jogador
        string nomeJogador = inputNome.text;

        if (string.IsNullOrWhiteSpace(nomeJogador))
        {
            nomeJogador = "Player";
        }

        // Código sala
        string nomeSala = inputEntrarSala.text;

        if (string.IsNullOrWhiteSpace(nomeSala))
        {
            Debug.Log("Digite o código da sala!");
            return;
        }

        // Salvar nome
        PlayerPrefs.SetString("PlayerName", nomeJogador);

        // Criar runner
        runner = Instantiate(runnerPrefab);

        runner.ProvideInput = true;

        // Entrar sala
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,

            SessionName = nomeSala,

            Scene = SceneRef.FromIndex(1),

            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),

            // NÃO cria sala automaticamente
            EnableClientSessionCreation = false
        });

        if (!result.Ok)
        {
            Debug.Log("Sala não existe!");
        }
    }

    // =====================================================
    // SAIR DO JOGO
    // =====================================================

    public void SairJogo()
    {
        Debug.Log("Saiu do jogo");

        Application.Quit();
    }
}