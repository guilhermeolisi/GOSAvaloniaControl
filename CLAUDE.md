# GOSAvaloniaControl: controles Avalonia personalizados

Controles de UI reutilizaveis (Avalonia 11.3, .NET 10) consumidos pelo Nimloth:
11 projetos entram no `Nimloth.sln` (GOSChartViewer, GOSImageViewer, GOSTextEditor,
GOSNavigationBar e Model, GOSNotification, Interface e Model, GOSBaseInsection,
TextMateModels, entre outros). Solucao propria: `GOS Avalonia Control.sln`.
Regras compartilhadas em `../CLAUDE.md`; regras de Avalonia/XAML em
`../.claude/rules/avalonia.md`.

## Pontos de atencao

- Submodulo git: `src/SindarinTextMate/Grammars/sindarin` aponta para
  `github.com/guilhermeolisi/sindarin-grammar`. Atualize com `git submodule update`.
- Nao ha testes. Testes novos vao em `test/<Projeto>.Tests` com
  `Avalonia.Headless.XUnit` 11.3.x (`[AvaloniaFact]`, paralelismo desligado).
- DI pelo `BaseLibrary.DependencyInjection`. Depende de `GOSAvaloniaServices`.

## Comandos

- `dotnet build "GOS Avalonia Control.sln" --no-restore`
- Consumidor: `dotnet build ../Nimloth/Nimloth.sln --no-restore`
