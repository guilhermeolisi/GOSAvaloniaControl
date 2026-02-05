//using AvaloniaEdit.TextMate.Resources;

using System.Diagnostics;
using System.Text.Json;
using TextMateSharp.Internal.Grammars.Reader;
using TextMateSharp.Internal.Themes.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace TextMate.Models;

public class RegistryOptions : IRegistryOptions
{

    ThemeName _defaultTheme;
    Dictionary<string, GrammarDefinition> _availableGrammars = new Dictionary<string, GrammarDefinition>();

    public RegistryOptions(ThemeName defaultTheme)
    {
        _defaultTheme = defaultTheme;
        InitializeAvailableGrammars();
    }

    //public List<Language> GetAvailableLanguages()
    //{
    //    List<Language> result = new List<Language>();

    //    foreach (GrammarDefinition definition in _availableGrammars.Values)
    //    {
    //        foreach (Language language in definition.Contributes.Languages)
    //        {
    //            if (language.Aliases == null || language.Aliases.Count == 0)
    //                continue;

    //            if (!HasGrammar(language.Id, definition.Contributes.Grammars))
    //                continue;

    //            result.Add(language);
    //        }
    //    }

    //    return result;
    //}

    public Language? GetLanguageByExtension(string extension)
    {
        foreach (GrammarDefinition definition in _availableGrammars.Values)
        {
            foreach (var language in definition.Contributes.Languages)
            {
                if (language.Extensions == null)
                    continue;

                foreach (var languageExtension in language.Extensions)
                {
                    if (extension.Equals(languageExtension,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return language;
                    }
                }
            }
        }

        return null;
    }

    public string? GetScopeByExtension(string extension)
    {
        foreach (GrammarDefinition definition in _availableGrammars.Values)
        {
            foreach (var language in definition.Contributes.Languages)
            {
                foreach (var languageExtension in language.Extensions)
                {
                    if (extension.Equals(languageExtension,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var grammar in definition.Contributes.Grammars)
                        {
                            return grammar.ScopeName;
                        }
                    }
                }
            }
        }

        return null;
    }

    public string? GetScopeByLanguageId(string languageId)
    {
        if (string.IsNullOrEmpty(languageId))
            return null;

        foreach (GrammarDefinition definition in _availableGrammars.Values)
        {
            foreach (var grammar in definition.Contributes.Grammars)
            {
                if (languageId.Equals(grammar.Language))
                    return grammar.ScopeName;
            }
        }

        return null;
    }

    public IRawTheme LoadTheme(ThemeName name)
    {
        return GetTheme(GetThemeFile(name));
    }

    #region IRegistryOptions
    //IRawTheme GetTheme(string scopeName);
    //IRawGrammar GetGrammar(string scopeName);
    //ICollection<string> GetInjections(string scopeName);
    //IRawTheme GetDefaultTheme();
    public ICollection<string>? GetInjections(string scopeName)
    {
        return null;
    }
    //public IRawTheme GetTheme(string scopeName)
    //{
    //    Stream themeStream = ResourceLoader.TryOpenThemeStream(scopeName.Replace("./", string.Empty));

    //    if (themeStream == null)
    //        return null;

    //    using (themeStream)
    //    using (StreamReader reader = new StreamReader(themeStream))
    //    {
    //        return ThemeReader.ReadThemeSync(reader);
    //    }
    //}

    public IRawTheme GetTheme(string scopeName)
    {
        using (StreamReader reader = ResourceLoader.OpenThemeStream(scopeName))
        {
#if DEBUG
            try
            {
                var theme = ThemeReader.ReadThemeSync(reader);
                return theme;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro no {nameof(GetTheme)}: " + ex.Message);
                throw;
                //return null;
            }
#endif
            return ThemeReader.ReadThemeSync(reader);
        }
    }

    //public IRawGrammar GetGrammar(string scopeName)
    //{
    //    Stream grammarStream = ResourceLoader.TryOpenGrammarStream(GetGrammarFile(scopeName));

    //    if (grammarStream == null)
    //        return null;

    //    using (grammarStream)
    //    using (StreamReader reader = new StreamReader(grammarStream))
    //    {
    //        return GrammarReader.ReadGrammarSync(reader);
    //    }
    //}
    public IRawGrammar GetGrammar(string scopeName)
    {
        //Stream grammarStream = ResourceLoader.TryOpenGrammarStream(GetGrammarFile(scopeName));

        using StreamReader reader = ResourceLoader.TryOpenGrammarStream(GetGrammarFile(scopeName));
        try
        {
#if DEBUG
            var grammar = GrammarReader.ReadGrammarSync(reader);
            //var grammar2 = GrammarReader.ReadGrammarSync(reader);
            return grammar; //Se eu tentar usar novamente o reader, ele vai dar erro
#endif

            return GrammarReader.ReadGrammarSync(reader);
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine($"Erro no {nameof(GetGrammar)}: " + ex.Message);
            //return null;
#endif
            throw;
        }
    }

    public IRawTheme GetDefaultTheme()
    {
        return LoadTheme(_defaultTheme);
    }

    #endregion
    //void InitializeAvailableGrammars()
    //{
    //    var serializer = new JsonSerializer();

    //    foreach (string grammar in GrammarNames.SupportedGrammars)
    //    {
    //        //using (Stream stream = ResourceLoader.OpenGrammarPackage(grammar))
    //        //using (StreamReader reader = new StreamReader(stream))
    //        using (StreamReader reader = ResourceLoader.OpenGrammarPackage(grammar))
    //        using (JsonTextReader jsonTextReader = new JsonTextReader(reader))
    //        {
    //            GrammarDefinition definition = serializer?.Deserialize<GrammarDefinition>(jsonTextReader)!;
    //            _availableGrammars.Add(grammar, definition);
    //        }
    //    }
    //}
    void InitializeAvailableGrammars()
    {
        //carregamento completo 74, 55, 45, 54ms
        //Sem carregar os arquivos de configuração e Snippets 34, 56, 44ms
        foreach (string grammar in GrammarNames.SupportedGrammars)
        {

            using (Stream stream = ResourceLoader.OpenGrammarPackageStream(grammar))
            {
                GrammarDefinition definition = JsonSerializer.Deserialize(stream, JsonSerializationContext.Default.GrammarDefinition)!;

                //Acredito que carregar os arquivos de configuração e snippets não seja necessário para o uso que faço, só serviria para aumentar o tempo de carregamento

                //                foreach (var language in definition.Contributes.Languages)
                //                {
                //                    language.Configuration = LanguageConfiguration.Load(grammar, language.ConfigurationFile);
                //#if DEBUG
                //                    if (language.Configuration is null)
                //                    {
                //                        Debug.WriteLine($"Configuration for {language.Id} is null");
                //                    }
                //#endif
                //                }

                //                definition.LanguageSnippets = LanguageSnippets.Load(grammar, definition.Contributes);

                _availableGrammars.Add(grammar, definition);
            }
        }
    }
    string GetGrammarFile(string scopeName)
    {
        foreach (string grammarName in _availableGrammars.Keys)
        {
            GrammarDefinition definition = _availableGrammars[grammarName];

            foreach (Grammar grammar in definition.Contributes.Grammars)
            {
                if (scopeName.Equals(grammar.ScopeName))
                {
                    string grammarPath = grammar.Path;

                    //if (grammarPath.StartsWith("./"))
                    //    grammarPath = grammarPath.Substring(2);
                    grammarPath = grammarPath.TrimStart('.').TrimStart('/');
                    //grammarPath = grammarPath.Replace("/", ".");
                    grammarPath = grammarPath.Replace('/', Path.DirectorySeparatorChar);

                    //return grammarName.ToLower() + "." + grammarPath;
                    return Path.Combine(grammarName.ToLower(), grammarPath);
                }
            }
        }

        return null;
    }

    string? GetThemeFile(ThemeName name)
    {
        switch (name)
        {
            case ThemeName.Abbys:
                return "abyss-color-theme.json";
            case ThemeName.Dark:
                return "dark_vs.json";
            case ThemeName.DarkPlus:
                return "dark_plus.json";
            case ThemeName.DimmedMonokai:
                return "dimmed-monokai-color-theme.json";
            case ThemeName.KimbieDark:
                return "kimbie-dark-color-theme.json";
            case ThemeName.Light:
                return "light_vs.json";
            case ThemeName.LightPlus:
                return "light_plus.json";
            case ThemeName.Monokai:
                return "monokai-color-theme.json";
            case ThemeName.QuietLight:
                return "quietlight-color-theme.json";
            case ThemeName.Red:
                return "Red-color-theme.json";
            case ThemeName.SolarizedDark:
                return "solarized-dark-color-theme.json";
            case ThemeName.SolarizedLight:
                return "solarized-light-color-theme.json";
            case ThemeName.TomorrowNightBlue:
                return "tomorrow-night-blue-color-theme.json";
            case ThemeName.HighContrastLight:
                return "hc_light.json";
            case ThemeName.HighContrastDark:
                return "hc_black.json";
        }

        return null;
    }
}
