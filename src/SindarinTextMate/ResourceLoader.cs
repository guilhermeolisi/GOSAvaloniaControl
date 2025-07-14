using System.Reflection;

namespace TextMate.Models;

internal class ResourceLoader
{
    static readonly string currentDirectory;
    //const string GrammarPrefix = "AvaloniaEdit.TextMate.Grammars.Resources.Grammars.";
    //const string ThemesPrefix = "AvaloniaEdit.TextMate.Grammars.Resources.Themes.";
    private const string GrammarPrefix = "Grammars";
    private const string ThemesPrefix = "Themes";
    static ResourceLoader()
    {
        currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)!;
    }
    internal static Stream OpenGrammarPackageStream(string grammarName)

    {
        // este é no original o internal static Stream OpenGrammarPackage
        //string grammarPackage = GrammarPrefix + grammarName.ToLowerInvariant() + "." + "package.json";

        //var result = typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream(
        //    grammarPackage);

        string grammarPackage = Path.Combine(currentDirectory, GrammarPrefix, grammarName.ToLower(), "package.json");

        var result = new FileStream(grammarPackage, FileMode.Open, FileAccess.Read); //Tenho que usar FileAccess.Read para não dar erro de permissão para abrir o arquivo que está no diretório de instalação do app do Windows Store 

        if (result == null)
            throw new FileNotFoundException("The grammar package '" + grammarPackage + "' was not found.");

        return result;
    }
    internal static StreamReader OpenGrammarPackage(string grammarName)
    {
        //string grammarPackage = GrammarPrefix + grammarName.ToLower() + "." + "package.json";
        //string grammarPackage = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), GrammarPrefix, grammarName.ToLower(), "package.json");
        string grammarPackage = Path.Combine(currentDirectory, GrammarPrefix, grammarName.ToLower(), "package.json");

        //var result = Stream typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream(
        //    grammarPackage);

#if DEBUG

        if (!File.Exists(grammarPackage))
        { }
        var trash = Directory.GetCurrentDirectory();
        var trash2 = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
#endif

        var result = new StreamReader(grammarPackage);

        if (result == null)
            throw new FileNotFoundException("The grammar package '" + grammarPackage + "' was not found.");

        return result;
    }
    internal static StreamReader OpenThemeStream(string themeFileName)
    {
        string themePackage = Path.Combine(currentDirectory, ThemesPrefix, themeFileName.ToLower());

        StreamReader result = new StreamReader(themePackage);
        //StreamReader result = new StreamReader(themePackage, System.Text.Encoding.UTF8);
        //StreamReader result = new StreamReader(themePackage, true);

        if (result is null)
            throw new FileNotFoundException("The grammar package '" + themePackage + "' was not found.");

        return result;
    }
    //internal static Stream TryOpenThemeStream(string path)
    //{
    //    return typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream(
    //        ThemesPrefix + path);
    //}

    internal static StreamReader TryOpenGrammarStream(string path)
    {
        //string grammarPackage = GrammarPrefix + grammarName.ToLower() + "." + "package.json";
        string grammarPackage = Path.Combine(currentDirectory, GrammarPrefix, path);

        //var result = Stream typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream(
        //    grammarPackage);

#if DEBUG
        if (!File.Exists(grammarPackage))
        { }
#endif

        var result = new StreamReader(grammarPackage);

        if (result == null)
            throw new FileNotFoundException("The grammar package '" + grammarPackage + "' was not found.");

        return result;
    }

    //internal static Stream TryOpenThemeStream(string path)
    //{
    //    return typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream(
    //        ThemesPrefix + path);
    //}

    //internal static Stream TryOpenLanguageConfiguration(string grammarName, string configurationFileName)
    //{
    //    configurationFileName = configurationFileName.Replace('/', '.').TrimStart('.');
    //    string grammarPackage = GrammarPrefix + grammarName.ToLowerInvariant() + "." + configurationFileName;

    //    var result = typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream(
    //        grammarPackage);

    //    return result;
    //}
    internal static Stream TryOpenLanguageConfiguration(string grammarName, string configurationFileName)
    {
        configurationFileName = configurationFileName.TrimStart('.').Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
        string grammarPackage = Path.Combine(currentDirectory, GrammarPrefix, grammarName.ToLowerInvariant(), configurationFileName);

        var result = new FileStream(grammarPackage, FileMode.Open, FileAccess.Read); //Tenho que usar FileAccess.Read para não dar erro de permissão para abrir o arquivo que está no diretório de instalação do app do Windows Store

        return result;
    }
    internal static Stream TryOpenLanguageSnippet(string grammarName, string snippetFileName)
    {
        //snippetFileName = snippetFileName.Replace('/', '.').TrimStart('.');
        //string snippetPackage = SnippetPrefix + grammarName.ToLowerInvariant() + "." + snippetFileName;
        snippetFileName = snippetFileName.TrimStart('.').Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
        string snippetPackage = Path.Combine(currentDirectory, GrammarPrefix, grammarName.ToLower(), snippetFileName);

        //var result = typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream(
        //    snippetPackage);

        var result = new FileStream(snippetPackage, FileMode.Open, FileAccess.Read); //Tenho que usar FileAccess.Read para não dar erro de permissão para abrir o arquivo que está no diretório de instalação do app do Windows Store

        //Pode ser que não exista o arquivo, então não é um erro fatal
        //if (result is null)
        //    throw new FileNotFoundException("The grammar package '" + snippetPackage + "' was not found.");

        return result;
    }
}
