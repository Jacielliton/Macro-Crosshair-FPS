Este é um programa desenvolvido em C# para criar uma sobreposição de mira em jogos, junto com um macro para controlar o recuo de diferentes armas. O programa permite selecionar entre três tipos de mira e cores, além de configurar macros para armas específicas, simulando o recuo durante os disparos.

## Funcionalidades

- **Seleção de Mira:** Escolha entre três tipos de mira: `Cross`(cruz), `Circle`(circulo), ou `Dot`(um ponto).
- **Cores da Mira:** Selecione a cor da mira entre `Red`, `Green`, `Blue`, `Yellow`, ou `White`.
- **Controle de Recuo:** O programa oferece macros de controle de recuo para três armas:
  - `SMG (Recuo Leve)`
  - `Rifle 556 (Recuo Médio)`
  - `Rifle 762 (Recuo Alto)`

## Instruções de Uso

1. **Selecione as Opções de Mira e Arma:**
   - Selecione a arma desejada, o tipo de mira e a cor da mira na interface principal.

2. **Iniciar o Macro:**
   - Pressione `Ctrl + Alt + I` para iniciar o macro. O macro funcionará enquanto o botão esquerdo do mouse estiver pressionado.

3. **Parar o Macro:**
   - Pressione `Ctrl + Alt + J` para parar o macro.

4. **Exibir a Mira:**
   - Clique em `Show Crosshair` para exibir a mira selecionada em uma sobreposição de tela cheia. A sobreposição é transparente e sobrepõe outros aplicativos, incluindo jogos.

5. **Fechar a Sobreposição:**
   - Pressione `Ctrl + Alt + K` para fechar a sobreposição.

## Compilação

### Pré-requisitos

- .NET SDK 8.0 ou superior. Você pode verificar se o .NET SDK está instalado com o comando:
  ```bash
  dotnet --version
