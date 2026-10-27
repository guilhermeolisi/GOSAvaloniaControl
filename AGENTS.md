# Instrucoes nativas do Codex

Fonte de sincronizacao: `GOSAvaloniaControl/CLAUDE.md`. Nao altere os originais Claude ao manter este porte.
Leia tambem os AGENTS.md ancestrais e o [AGENTS.md do workspace](../AGENTS.md), mesmo se a sessao iniciou neste repositorio Git filho.

Estas instrucoes sao completas para o Codex. Nao e necessario abrir o CLAUDE.md
original para segui-las. As adaptacoes abaixo prevalecem sobre mecanismos do Claude
descritos no conteudo sincronizado: ferramentas, modelos, cache, limites de contexto,
permissoes, plugins, metricas e hooks precisam de suporte verificado no cliente Codex.
Herde o modelo da sessao, sem aplicar aliases ou guardas Anthropic. Use apply_patch
para editar, rg para buscar e PowerShell para comandos. Leia regras paths explicitamente.
O estado e .codex/state/, separado do Claude. Use o id real CODEX_THREAD_ID e -Sessao
quando aceito; nunca reutilize marcadores de autorizacao ou revisao de outra sessao.
Para subagentes, use colaboracao nativa e contexto independente na revisao.
Hooks locais so executam em clientes locais apos revisao e confianca do usuario.
Em sessao orquestrada, cumpra as verificacoes explicitamente. A ausencia de hooks
nao dispensa autorizacao de linguagem, testes, revisao, rede ou horario de push.
Nao presuma LSP ou plugins disponiveis; confira ferramentas, XAML e linker.xml.
Comandos que iniciam claude ou dependem dele sao especificos do cliente Claude;
nao os execute como inicializacao do Codex.
O conteudo sincronizado abaixo preserva as regras de negocio e autorizacao.
Se a injecao destas instrucoes nao terminar no comentario CODEX-INSTRUCTIONS-END,
leia este AGENTS.md completo pelo sistema de arquivos antes de trabalhar. Uma sessao
ja aberta ou orquestrada pode manter um limite menor que o configurado no projeto.

## Instrucoes sincronizadas

# GOSAvaloniaControl: controles Avalonia personalizados

Controles de UI reutilizaveis (Avalonia 12.1, .NET 10) consumidos pelo Nimloth:
11 projetos entram no `Nimloth.sln` (GOSChartViewer, GOSImageViewer, GOSTextEditor,
GOSNavigationBar e Model, GOSNotification, Interface e Model, GOSBaseInsection,
TextMateModels, entre outros). Solucao propria: `GOS Avalonia Control.sln`.
Regras compartilhadas em `../AGENTS.md`; regras de Avalonia/XAML em
`../.codex/rules/avalonia.md`.

## Pontos de atencao

- Submodulo git: `src/SindarinTextMate/Grammars/sindarin` aponta para
  `github.com/guilhermeolisi/sindarin-grammar`. Quem avanca o ponteiro e o script do workspace,
  `.codex/ferramentas/sincronizar-gramatica.ps1` (`-Check` confere, `-SoPonteiros` so avanca); nao edite nada ali
  dentro e nao avance a mao.
- Testes em `test/<Projeto>.Tests` com `Avalonia.Headless.XUnit` 12.1.x em xunit v3 (`[AvaloniaFact]`,
  paralelismo desligado). O primeiro e o modelo: `test/GOSCustomControl.Tests` (E047), com versoes
  literais de pacote (o repositorio nao tem gestao central) e uma aplicacao headless sem tema.
- DI pelo `BaseLibrary.DependencyInjection`. Depende de `GOSAvaloniaServices`.

## Comandos

- `dotnet build "GOS Avalonia Control.sln" --no-restore`
- `dotnet test test/GOSCustomControl.Tests --no-restore`
- Consumidor: `dotnet build ../Nimloth/Nimloth.sln --no-restore`

<!-- CODEX-INSTRUCTIONS-END -->
