using System;
using System.IO;
using System.Reflection;

namespace Nimloth.TextMate.Models
{
    internal class ResourceLoader
    {
        static readonly string currentDirectory;
        //const string GrammarPrefix = "AvaloniaEdit.TextMate.Grammars.Resources.Grammars.";
        //const string ThemesPrefix = "AvaloniaEdit.TextMate.Grammars.Resources.Themes.";
        const string GrammarPrefix = "Grammars";
        const string ThemesPrefix = "Themes";

        static ResourceLoader()
        {
            currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
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
            var trash2 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif

            var result = new StreamReader(grammarPackage);

            if (result == null)
                throw new FileNotFoundException("The grammar package '" + grammarPackage + "' was not found.");

            return result;
        }
        internal static StreamReader OpenThemeStream(string themeFileName)
        {
            string themePackage = Path.Combine(currentDirectory, ThemesPrefix, themeFileName.ToLower());

            var result = new StreamReader(themePackage);

            if (result == null)
                throw new FileNotFoundException("The grammar package '" + themePackage + "' was not found.");

            return result;
        }
        //internal static Stream TryOpenGrammarStream(string path)
        //{
        //    return typeof(ResourceLoader).GetTypeInfo().Assembly.GetManifestResourceStream(
        //        GrammarPrefix + path);
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
    }
}
