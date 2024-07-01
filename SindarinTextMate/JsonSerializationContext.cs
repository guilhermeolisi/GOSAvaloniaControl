using System.Text.Json.Serialization;

namespace TextMate.Models;

[JsonSerializable(typeof(GrammarDefinition))]
[JsonSerializable(typeof(LanguageSnippets))]
[JsonSerializable(typeof(LanguageSnippet))]
[JsonSerializable(typeof(LanguageConfiguration))]
[JsonSerializable(typeof(EnterRule))]
[JsonSerializable(typeof(AutoPair))]
[JsonSerializable(typeof(IList<string>))]
internal sealed partial class JsonSerializationContext : JsonSerializerContext
{
}