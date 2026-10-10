# GOSAvaloniaControl: controles Avalonia personalizados

Controles de UI reutilizaveis (Avalonia 11.3, .NET 10) consumidos pelo Nimloth:
11 projetos entram no `Nimloth.sln` (GOSChartViewer, GOSImageViewer, GOSTextEditor,
GOSNavigationBar e Model, GOSNotification, Interface e Model, GOSBaseInsection,
TextMateModels, entre outros). Solucao propria: `GOS Avalonia Control.sln`.
Regras compartilhadas em `../CLAUDE.md`; regras de Avalonia/XAML em
`../.claude/rules/avalonia.md`.

## Pontos de atencao

- Submodulo git: `src/SindarinTextMate/Grammars/sindarin` aponta para
  `github.com/guilhermeolisi/sindarin-grammar`. Quem avanca o ponteiro e o script do workspace,
  `.claude/ferramentas/sincronizar-gramatica.ps1` (`-Check` confere, `-SoPonteiros` so avanca); nao edite nada ali
  dentro e nao avance a mao.
- Testes em `test/<Projeto>.Tests` com `Avalonia.Headless.XUnit` 11.3.x (`[AvaloniaFact]`,
  paralelismo desligado). O primeiro e o modelo: `test/GOSCustomControl.Tests` (E047), com versoes
  literais de pacote (o repositorio nao tem gestao central) e uma aplicacao headless sem tema.
- DI pelo `BaseLibrary.DependencyInjection`. Depende de `GOSAvaloniaServices`.

## Comandos

- `dotnet build "GOS Avalonia Control.sln" --no-restore`
- `dotnet test test/GOSCustomControl.Tests --no-restore`
- Consumidor: `dotnet build ../Nimloth/Nimloth.sln --no-restore`
