# Design das Telas Iniciais - Pet Cuttie Games

## 1. Objetivo

Definir o fluxo, layout e implementação das telas que antecedem a partida:

1. **Splash / Loading**
2. **Tela Inicial (Main Menu)**
3. **Seleção de Instrumento**
4. **Seleção de Música**
5. **Seleção de Dificuldade**

O documento serve como guia prático de implementação no Unity 2022.3 LTS, reaproveitando os sistemas já existentes (`GameManager`, `NoteSpawner`, `AudioManager`, `ScoreManager`).

---

## 2. Fluxo de Telas

```
[Splash/Loading]
      |
      v
[Tela Inicial] ----> [Configurações]* ----> [Créditos]*
      |
      v
[Seleção de Instrumento]
      |
      v
[Seleção de Música]
      |
      v
[Seleção de Dificuldade]
      |
      v
[Gameplay Scene]
```

\* *Telas opcionais para versão inicial do protótipo.*

---

## 3. Decisão de Arquitetura

**Recomendação: duas cenas separadas.**

| Cena | Responsabilidade |
|------|------------------|
| `Menu.unity` | Todas as telas iniciais (painéis UI alternados) |
| `Gameplay.unity` | Partida em si (já previsto no README) |

**Motivo:** mantém a cena de gameplay limpa e permite que o `GameManager` existente continue funcionando com poucas alterações. As telas iniciais ficam em uma cena dedicada, gerenciadas por um `MenuManager`.

---

## 4. Especificação das Telas

### 4.1 Splash / Loading

**Objetivo:** exibir logo/marca e garantir que os sistemas singletons (`AudioManager`, `ScoreManager`) estejam prontos.

| Propriedade | Valor |
|-------------|-------|
| Duração mínima | 2.0 segundos |
| Fundo | Cor sólida `#1A1A2E` (mesma da câmera do gameplay) |
| Elementos | Logo centralizado, barra de progresso simples (opcional) |
| Transição automática | Sim, após carregar dados assíncronos |

**Comportamento:**
- Mostra logo.
- Carrega lista de instrumentos e músicas via `Resources`.
- Após 2s + fim do carregamento, avança para a Tela Inicial.

### 4.2 Tela Inicial (Main Menu)

**Objetivo:** porta de entrada do jogo.

**Layout (Canvas `Screen Space - Overlay`, referência 1920x1080):**

```
+------------------------------------------+
|                                          |
|          [ LOGO / TÍTULO ]              |
|                                          |
|                                          |
|             [ JOGAR ]                    |
|                                          |
|          [ CONFIGURAÇÕES ]*              |
|                                          |
|            [ CRÉDITOS ]*                 |
|                                          |
+------------------------------------------+
```

| Elemento | Tipo | Ação |
|----------|------|------|
| Título | TextMeshPro | Texto "Pet Cuttie Games" |
| Botão Jogar | Button + TMP | Vai para `Seleção de Instrumento` |
| Botão Configurações | Button + TMP | *(opcional)* Abre painel de volume |
| Botão Créditos | Button + TMP | *(opcional)* Abre painel de créditos |

### 4.3 Seleção de Instrumento

**Objetivo:** escolher o timbre que o jogador ouvirá ao acertar notas.

**Instrumentos iniciais:**

| ID | Nome | Descrição do som | Alteração real no AudioManager |
|----|------|------------------|-------------------------------|
| `Piano` | Piano | Som suave com ataque rápido | Harmônicos atuais (1x, 2x, 3x, 4x) |
| `Synth` | Synth | Onda mais pura, timbre retrô | Menos harmônicos, mais senoidal |
| `Pluck` | Pluck | Som curto e percussivo | `noteDuration` menor, decay rápido |

**Layout:**

```
+------------------------------------------+
|        ESCOLHA SEU INSTRUMENTO           |
|                                          |
|   [Piano]     [Synth]     [Pluck]        |
|   (imagem)   (imagem)    (imagem)        |
|                                          |
|   [  ← VOLTAR ]    [ JOGAR → ]           |
+------------------------------------------+
```

| Elemento | Comportamento |
|----------|---------------|
| Cards de instrumento | Botões grandes; ao selecionar, destaca borda/cor |
| Botão Voltar | Retorna para Tela Inicial |
| Botão Jogar | Avança para Seleção de Música |

### 4.4 Seleção de Música

**Objetivo:** escolher a música a ser tocada.

**Fonte de dados:** arquivos JSON em `Assets/Resources/Songs/*.json`.

**Músicas iniciais:**

| Arquivo | Título |Compositor | BPM | Dificuldade base |
|---------|--------|-----------|-----|------------------|
| `OdeToJoy.json` | Ode to Joy | Beethoven | 100 | Easy |

**Layout:**

```
+------------------------------------------+
|         ESCOLHA A MÚSICA                 |
|                                          |
|  +----------------------------------+    |
|  | Ode to Joy  |  Beethoven  | 100BPM |  |
|  +----------------------------------+    |
|  | Próxima música desbloqueada...   |   |
|  +----------------------------------+    |
|                                          |
|  [  ← VOLTAR ]    [ PRÓXIMA → ]          |
+------------------------------------------+
```

| Elemento | Comportamento |
|----------|---------------|
| Item da lista | Mostra título, compositor, BPM; ao clicar, seleciona |
| ScrollRect | Lista vertical quando houver muitas músicas |
| Indicador de seleção | Ícone/check ao lado do item ativo |

### 4.5 Seleção de Dificuldade

**Objetivo:** escolher o nível de desafio, que afeta a velocidade das notas e a tolerância de acerto.

| Nível | Multiplicador de velocidade | Tolerância de acerto | Fator de score |
|-------|----------------------------|----------------------|----------------|
| Iniciante | 0.6x | Larga (+30%) | 0.8x |
| Fácil | 0.8x | Larga | 1.0x |
| Médio | 1.0x | Padrão | 1.2x |
| Difícil | 1.3x | Rigorosa (-30%) | 1.5x |
| Expert | 1.6x | Muito rigorosa | 2.0x |

**Layout:**

```
+------------------------------------------+
|      ESCOLHA A DIFICULDADE                 |
|                                          |
|         [ INICIANTE ]                    |
|         [   FÁCIL   ]                    |
|         [   MÉDIO   ]                    |
|         [  DIFÍCIL  ]                    |
|         [  EXPERT   ]                    |
|                                          |
|  [  ← VOLTAR ]    [  INICIAR  ]          |
+------------------------------------------+
```

| Elemento | Comportamento |
|----------|---------------|
| Botões de dificuldade | Ao selecionar, destaca o nível |
| Botão Iniciar | Carrega `Gameplay.unity` passando os dados escolhidos |

---

## 5. Estrutura de Dados

### 5.1 Instrumento

```csharp
[System.Serializable]
public class InstrumentData
{
    public string id;
    public string displayName;
    public Sprite icon;
    public float noteDuration;
    public float attackTime;
    public float decayTime;
    public float sustainLevel;
    public float releaseTime;
    public AnimationCurve harmonicWeights;
}
```

### 5.2 Música

Já existe uma estrutura implícita no `NoteSpawner`. Padronizar para:

```csharp
[System.Serializable]
public class SongData
{
    public string title;
    public string composer;
    public float bpm;
    public string difficulty;
    public List<SongNote> notes;
}
```

O JSON `OdeToJoy.json` já segue esse formato.

### 5.3 Configuração de Dificuldade

```csharp
[System.Serializable]
public class DifficultyConfig
{
    public string id;
    public string displayName;
    public float speedMultiplier = 1f;
    public float hitWindowMultiplier = 1f;
    public float scoreMultiplier = 1f;
}
```

### 5.4 Dados da Sessão

Classe singleton que persiste entre as cenas `Menu` e `Gameplay`:

```csharp
public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public string SelectedInstrumentId { get; set; }
    public string SelectedSongId { get; set; }
    public string SelectedDifficultyId { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

---

## 6. Arquitetura de Navegação

### 6.1 MenuManager

Gerencia os painéis dentro da cena `Menu.unity`.

```csharp
public class MenuManager : MonoBehaviour
{
    [Header("Telas")]
    [SerializeField] private GameObject splashPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject instrumentPanel;
    [SerializeField] private GameObject songPanel;
    [SerializeField] private GameObject difficultyPanel;

    public void ShowMainMenu() => ShowPanel(mainMenuPanel);
    public void ShowInstrumentSelection() => ShowPanel(instrumentPanel);
    public void ShowSongSelection() => ShowPanel(songPanel);
    public void ShowDifficultySelection() => ShowPanel(difficultyPanel);

    private void ShowPanel(GameObject panel)
    {
        splashPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        instrumentPanel.SetActive(false);
        songPanel.SetActive(false);
        difficultyPanel.SetActive(false);

        panel.SetActive(true);
    }
}
```

### 6.2 InstrumentSelection

```csharp
public class InstrumentSelection : MonoBehaviour
{
    [SerializeField] private List<InstrumentData> instruments;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject instrumentButtonPrefab;

    private InstrumentData selectedInstrument;

    private void Start()
    {
        BuildInstrumentList();
    }

    private void BuildInstrumentList()
    {
        foreach (var instrument in instruments)
        {
            var button = Instantiate(instrumentButtonPrefab, buttonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = instrument.displayName;
            button.GetComponent<Button>().onClick.AddListener(() => SelectInstrument(instrument));
        }
    }

    private void SelectInstrument(InstrumentData instrument)
    {
        selectedInstrument = instrument;
        GameSession.Instance.SelectedInstrumentId = instrument.id;
    }
}
```

### 6.3 SongSelection

```csharp
public class SongSelection : MonoBehaviour
{
    [SerializeField] private Transform listContainer;
    [SerializeField] private GameObject songItemPrefab;

    private List<SongData> songs = new List<SongData>();
    private SongData selectedSong;

    private void Start()
    {
        LoadSongs();
        BuildSongList();
    }

    private void LoadSongs()
    {
        // Carrega todos os JSONs da pasta Resources/Songs
        TextAsset[] songAssets = Resources.LoadAll<TextAsset>("Songs");
        foreach (var asset in songAssets)
        {
            songs.Add(JsonUtility.FromJson<SongData>(asset.text));
        }
    }

    private void BuildSongList()
    {
        foreach (var song in songs)
        {
            var item = Instantiate(songItemPrefab, listContainer);
            item.GetComponentInChildren<TextMeshProUGUI>().text = $"{song.title} - {song.composer}";
            item.GetComponent<Button>().onClick.AddListener(() => SelectSong(song));
        }
    }

    private void SelectSong(SongData song)
    {
        selectedSong = song;
        GameSession.Instance.SelectedSongId = song.title; // ou um campo "id" futuro
    }
}
```

### 6.4 DifficultySelection

```csharp
public class DifficultySelection : MonoBehaviour
{
    [SerializeField] private List<DifficultyConfig> difficulties;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject difficultyButtonPrefab;

    private DifficultyConfig selectedDifficulty;

    private void Start()
    {
        BuildDifficultyList();
    }

    private void BuildDifficultyList()
    {
        foreach (var difficulty in difficulties)
        {
            var button = Instantiate(difficultyButtonPrefab, buttonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = difficulty.displayName;
            button.GetComponent<Button>().onClick.AddListener(() => SelectDifficulty(difficulty));
        }
    }

    private void SelectDifficulty(DifficultyConfig difficulty)
    {
        selectedDifficulty = difficulty;
        GameSession.Instance.SelectedDifficultyId = difficulty.id;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
```

---

## 7. Integração com Sistemas Existentes

### 7.1 NoteSpawner

O `NoteSpawner` mantém `OdeToJoy` como fallback no `Awake`, e expõe métodos para carregar uma música e aplicar dificuldade a partir do menu:

```csharp
public void LoadSong(SongData song)
{
    if (song == null) return;
    songNotes = song.ToSongNotes();
}

public void ApplyDifficulty(DifficultyConfig difficulty)
{
    if (difficulty == null) return;
    fallSpeed *= difficulty.speedMultiplier;
}
```

No `GameManager`, o fluxo de integração usa `ApplySessionChoices()` e `autoStartFromMenu`:

```csharp
[SerializeField] private bool autoStartFromMenu = true;

private void Start()
{
    ApplySessionChoices();

    if (autoStartFromMenu && GameSession.Instance != null)
    {
        StartGame();
    }
    else
    {
        ShowStartScreen();
    }
}

private void ApplySessionChoices()
{
    if (GameSession.Instance == null)
    {
        Debug.Log("GameManager: GameSession nao encontrado. Usando valores padrao.");
        return;
    }

    SongData song = SongLoader.Load(GameSession.Instance.SelectedSongId);
    noteSpawner?.LoadSong(song);

    DifficultyConfig difficulty = Resources.Load<DifficultyConfig>(
        $"Difficulties/{GameSession.Instance.SelectedDifficultyId}");
    if (difficulty != null)
    {
        noteSpawner?.ApplyDifficulty(difficulty);
        ApplyDifficultyToLanes(difficulty.hitWindowMultiplier);
    }

    InstrumentData instrument = Resources.Load<InstrumentData>(
        $"Instruments/{GameSession.Instance.SelectedInstrumentId}");
    if (instrument != null)
    {
        audioManager?.ApplyInstrument(instrument);
    }
}
```

### 7.2 AudioManager

Adicionar método para aplicar configurações de instrumento:

```csharp
public void ApplyInstrument(InstrumentData instrument)
{
    noteDuration = instrument.noteDuration;
    attackTime = instrument.attackTime;
    decayTime = instrument.decayTime;
    sustainLevel = instrument.sustainLevel;
    releaseTime = instrument.releaseTime;
    GenerateNoteClips();
}
```

### 7.3 Lane

Para aplicar a dificuldade na tolerância de acerto, adicionar:

```csharp
public void ApplyDifficulty(float hitWindowMultiplier)
{
    perfectThreshold *= hitWindowMultiplier;
    goodThreshold *= hitWindowMultiplier;
}
```

---

## 8. Guia de Implementação Passo a Passo

### 8.1 Criar a cena de Menu

1. No Unity, vá em **File > New Scene**.
2. Salve como `Assets/Scenes/Menu.unity`.
3. Crie um **Canvas**: `GameObject > UI > Canvas`.
   - **Render Mode**: `Screen Space - Overlay`
   - **Canvas Scaler**: `Scale With Screen Size`, Reference Resolution `1920x1080`
4. Crie um objeto vazio chamado **MenuManager** na raiz da cena.
5. Adicione o componente `MenuManager.cs`.
6. Crie 5 painéis filhos do Canvas:
   - `SplashPanel`
   - `MainMenuPanel`
   - `InstrumentPanel`
   - `SongPanel`
   - `DifficultyPanel`
7. Arraste cada painel para os campos correspondentes do `MenuManager`.

### 8.2 Criar o GameSession

1. Crie `Assets/Scripts/GameSession.cs` com o código da seção 5.4.
2. Na cena `Menu`, crie um objeto vazio `GameSession`.
3. Adicione o componente `GameSession.cs`.

### 8.3 Configurar Build Settings

1. Vá em **File > Build Settings**.
2. Adicione as cenas na ordem:
   - `Assets/Scenes/Menu.unity`
   - `Assets/Scenes/Gameplay.unity`

### 8.4 Criar a Tela Inicial

1. Dentro de `MainMenuPanel`:
   - Adicione um `TextMeshPro - Text` para o título.
   - Adicione 3 botões: Jogar, Configurações, Créditos.
2. No botão **Jogar**, conecte o evento `OnClick` para `MenuManager.ShowInstrumentSelection()`.

### 8.5 Criar a Seleção de Instrumento

1. Dentro de `InstrumentPanel`:
   - Crie um `GridLayoutGroup` (3 colunas, padding 50).
   - Crie um prefab de botão de instrumento (`Assets/Prefabs/InstrumentButton.prefab`).
2. Crie `Assets/Scripts/InstrumentSelection.cs` e adicione ao `InstrumentPanel`.
3. Configure a lista de `InstrumentData` no Inspector.
4. Botão **Voltar**: arraste o botão para o campo `Back Button` e conecte `OnClick` a `InstrumentSelection.OnBack()`.
5. Botão **Próximo**: arraste o botão para o campo `Confirm Button` e conecte `OnClick` a `InstrumentSelection.OnConfirm()`.

### 8.6 Criar a Seleção de Música

1. Dentro de `SongPanel`:
   - Crie um `Scroll View` (`GameObject > UI > Scroll View`).
   - No `Content`, adicione um `VerticalLayoutGroup`.
2. Crie o prefab `SongItem.prefab` com botão + texto.
3. Crie `Assets/Scripts/SongSelection.cs` e adicione ao `SongPanel`.
4. Botão **Voltar**: arraste o botão para o campo `Back Button` e conecte `OnClick` a `SongSelection.OnBack()`.
5. Botão **Próximo**: arraste o botão para o campo `Confirm Button` e conecte `OnClick` a `SongSelection.OnConfirm()`.

### 8.7 Criar a Seleção de Dificuldade

1. Dentro de `DifficultyPanel`:
   - Crie um `VerticalLayoutGroup` com 5 botões.
2. Crie `Assets/Scripts/DifficultySelection.cs`.
3. Configure a lista de `DifficultyConfig` no Inspector.
4. Botão **Voltar**: arraste o botão para o campo `Back Button` e conecte `OnClick` a `DifficultySelection.OnBack()`.
5. Botão **Iniciar**: arraste o botão para o campo `Start Button` e conecte `OnClick` a `DifficultySelection.StartGame()`.

### 8.8 Adaptar GameplayScene

1. Garanta que `Gameplay.unity` tenha o `GameManager`, `NoteSpawner`, lanes, `AudioManager` e `ScoreManager`.
2. No `GameManager`, marque `Auto Start From Menu = true` quando a cena for carregada a partir do menu. Com essa opção ativada, a partida inicia automaticamente após `ApplySessionChoices()`.
3. Quando `GameSession` não existir (por exemplo, testando `Gameplay.unity` isoladamente no editor), o `GameManager` exibe um log e usa o fallback do `NoteSpawner`, que carrega `OdeToJoy` no `Awake`.

> **Nota importante sobre assets:**
> - O `GameManager` busca `DifficultyConfig` e `InstrumentData` via `Resources.Load` nas pastas `Resources/Difficulties/` e `Resources/Instruments/`. Certifique-se de que os ScriptableObjects criados no editor estejam nessas pastas e que seus `id` correspondam aos valores salvos no `GameSession`.
> - Os scripts `InstrumentSelection` e `DifficultySelection` usam listas configuradas no Inspector, então os mesmos assets devem ser arrastados para os campos `Instruments` e `Difficulties` respectivamente. O `SongSelection` carrega músicas dinamicamente de `Resources/Songs/`.
> - Se os assets não estiverem em `Resources`, a dificuldade e o instrumento não serão aplicados, mas a música padrão ainda funcionará.

---

## 9. Checklist de Validação

- [x] Scripts `MenuManager`, `GameSession`, `InstrumentSelection`, `SongSelection`, `DifficultySelection` e `GameManager` implementados e compilando.
- [x] Splash avança automaticamente para o Main Menu (`MenuManager`).
- [x] Métodos de navegação (`ShowMainMenu`, `ShowInstrumentSelection`, etc.) implementados.
- [x] Instrumento selecionado é salvo no `GameSession`.
- [x] Música selecionada é salva no `GameSession`.
- [x] Dificuldade selecionada é salva no `GameSession`.
- [x] Botão "Iniciar" carrega `Gameplay.unity` (`DifficultySelection.StartGame`).
- [x] `Gameplay.unity` usa a música e dificuldade escolhidas (`GameManager.ApplySessionChoices`).
- [x] Botões de "Voltar" implementados em todas as telas de seleção (`OnBack`).
- [ ] Cena `Menu.unity` montada no Unity com painéis e prefabs.
- [ ] Build Settings incluem `Menu` e `Gameplay` na ordem correta.
- [ ] Assets `InstrumentData` e `DifficultyConfig` criados via menu do editor e referenciados nos scripts de seleção.
- [ ] Sons/efeitos de UI e transições visuais entre painéis.

---

## 10. Próximos Passos Sugeridos

Após implementar as telas iniciais:

- [ ] Tela de resultados com estatísticas detalhadas.
- [ ] Sistema de desbloqueio de músicas.
- [ ] Tela de configurações (volume, controles).
- [ ] Animações e transições entre painéis.
- [ ] Suporte a navegação por teclado e controle.
