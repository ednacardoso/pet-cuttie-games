# Pet Cuttie Games - Prototipo de Jogo Musical

Prototipo inicial de um app de musica interativo estilo Guitar Hero, com foco em mecanica de notas, pontuacao e som.

## Tecnologias

- **Unity 2022.3 LTS**
- **C#**
- **Visual Studio Code** (com extensao C# e extensao Unity)

## Estrutura do Projeto

```
Assets/
  Scripts/        # Codigo C# do jogo
  Prefabs/        # Prefabs do Unity (criados no editor)
  Scenes/         # Cenas do Unity
  Resources/      # Dados como JSON de musicas
  Audio/          # Arquivos de audio (a adicionar)
ProjectSettings/  # Configuracoes do projeto
Packages/         # Pacotes Unity
```

## Como abrir o projeto

1. Instale o **Unity Hub** e o **Unity 2022.3 LTS** (ou compativel).
2. No Unity Hub, clique em **Open** e selecione a pasta deste projeto.
3. Aguarde o Unity importar os pacotes.

## Fluxo de Telas Iniciais

A cena `Menu.unity` (a criar no Unity) gerencia o fluxo:

```
Splash -> Main Menu -> Selecao de Instrumento -> Selecao de Musica -> Selecao de Dificuldade -> Gameplay
```

Scripts prontos:

- `MenuManager.cs`: controla qual painel esta ativo.
- `GameSession.cs`: singleton persistente entre cenas; guarda `SelectedInstrumentId`, `SelectedSongId` e `SelectedDifficultyId`.
- `InstrumentSelection.cs`: constroi a lista de instrumentos a partir de `ScriptableObject`s arrastados no Inspector.
- `SongSelection.cs`: carrega musicas de `Assets/Resources/Songs/*.json`.
- `DifficultySelection.cs`: constroi a lista de dificuldades a partir de `ScriptableObject`s arrastados no Inspector.

Cada tela de selecao possui metodos `OnConfirm()` e `OnBack()` que podem ser ligados a botoes no Inspector.

Na cena `Gameplay`, o `GameManager` le os valores do `GameSession` e aplica musica, dificuldade e instrumento. Se `GameSession` nao existir (teste direto na cena), `NoteSpawner` mantem a musica padrao `OdeToJoy`.

> **Ainda necessario no Unity:** criar os paineis/prefabs, arrastar os assets `InstrumentData` e `DifficultyConfig` nos campos do Inspector, configurar os botoes `OnClick` e adicionar as cenas `Menu` e `Gameplay` em **File > Build Settings**.

## Configuracao da cena (passo a passo)

### 1. Criar a cena principal

1. No Unity, va em **File > New Scene**.
2. Salve como `Assets/Scenes/Gameplay.unity`.

### 2. Configurar a camera

1. Selecione a **Main Camera**.
2. No Inspector, configure:
   - **Projection**: Orthographic
   - **Size**: 5
   - **Position**: (0, 0, -10)
   - **Background**: cor escura (ex: `#1A1A2E`)

### 3. Criar as lanes (pistas)

1. Crie 5 objetos vazios filhos da cena: **Lane0**, **Lane1**, **Lane2**, **Lane3**, **Lane4**.
2. Posicione-os em X: `-4`, `-2`, `0`, `2`, `4` (Y: `4` para spawn, `Y: -3` para zona de acerto).
3. Em cada Lane, adicione o componente **Lane** (`Assets/Scripts/Lane.cs`).
4. Configure cada Lane:
   - **Lane Index**: 0, 1, 2, 3, 4
   - **Keyboard Key**: A, S, D, F, G
   - **Hit Zone**: crie um objeto vazio filho em Y = -3 e arraste para o campo
   - Adicione um **BoxCollider2D** com `Is Trigger = true` na zona de acerto

### 4. Criar o prefab da nota

1. Crie um sprite: **GameObject > 2D Object > Sprites > Square**.
2. Renomeie para `NotePrefab`.
3. Adicione os componentes:
   - **Note** (`Assets/Scripts/Note.cs`)
   - **BoxCollider2D** com `Is Trigger = true`
4. Arraste o objeto para a pasta `Assets/Prefabs/` para criar o prefab.
5. Delete o objeto da cena.

### 5. Criar os objetos de gerenciamento

1. Crie um objeto vazio chamado **GameManager**.
2. Adicione o componente **GameManager** (`Assets/Scripts/GameManager.cs`).
3. Crie um objeto vazio chamado **ScoreManager**.
4. Adicione o componente **ScoreManager** (`Assets/Scripts/ScoreManager.cs`).
5. Crie um objeto vazio chamado **NoteSpawner**.
6. Adicione o componente **NoteSpawner** (`Assets/Scripts/NoteSpawner.cs`).
7. Crie um objeto vazio chamado **AudioManager**.
8. Adicione o componente **AudioManager** (`Assets/Scripts/AudioManager.cs`).

### 6. Conectar as referencias

**NoteSpawner:**
- **Note Prefab**: arraste o prefab `NotePrefab`
- **Lane Spawn Points**: arraste os 5 objetos Lane (posicao Y = 4)

**GameManager:**
- **Note Spawner**: arraste o objeto NoteSpawner
- **Score Manager**: arraste o objeto ScoreManager

**ScoreManager (opcional, para UI):**
1. Crie um Canvas: **GameObject > UI > Canvas**.
2. Mude o **Render Mode** para **Screen Space - Camera** e arraste a Main Camera.
3. Adicione 3 textos (TextMeshPro):
   - `ScoreText` com texto "Score: 000000"
   - `ComboText` com texto vazio
   - `FeedbackText` com texto vazio
4. Arraste cada um nos campos correspondentes do ScoreManager.

### 7. Testar

1. Aperte **Play** no Unity.
2. Aperte **ESPACO** para iniciar.
3. Toque as notas com **A, S, D, F, G** conforme chegam na zona de acerto.

## Audio

O projeto gera os sons proceduralmente, sem depender de arquivos de audio externos.

- Cada lane (C, D, E, F, G) possui uma frequencia correspondente.
- Ao acertar uma nota, o `AudioManager` toca o som sintetizado da nota.
- Ao errar, toca um som de ruido curto.

### Configuracao do AudioManager

1. Crie um objeto vazio chamado **AudioManager** na cena.
2. Adicione o componente `Assets/Scripts/AudioManager.cs`.
3. Ajuste no Inspector (opcional):
   - **Master Volume**: volume geral (0 a 1)
   - **Note Duration**: duracao de cada nota
   - **Attack/Decay/Sustain/Release**: formato do envelope do som

O `AudioManager` e um singleton e funciona automaticamente quando uma nota e acertada ou perdida.

## Usando o Visual Studio Code

1. Va em **Edit > Preferences > External Tools** no Unity.
2. Em **External Script Editor**, escolha **Visual Studio Code**.
3. Abra qualquer script no Unity — ele abrira no VS Code.
4. Instale as extensoes no VS Code:
   - C# (Microsoft)
   - Unity (Microsoft)

## Proximos Passos

- [x] Adicionar audio para as notas (sintese procedural)
- [x] Criar tela de selecao de instrumento (scripts prontos; configurar no Unity)
- [x] Criar tela de selecao de musica (scripts prontos; configurar no Unity)
- [x] Implementar niveis de dificuldade (scripts prontos; criar assets no Unity)
- [ ] Sistema de ranking e pontuacao
- [ ] Modo multiplayer com Photon PUN 2
- [ ] Importar musicas classicas via MIDI

## Musica Incluida

- **Ode to Joy** (An die Freude) - Ludwig van Beethoven
- Dados em: `Assets/Resources/Songs/OdeToJoy.json`

## Notas

Este e um prototipo funcional. A musica esta carregada diretamente no codigo do `NoteSpawner`. Futuramente podemos criar um loader generico de JSON/MIDI.
