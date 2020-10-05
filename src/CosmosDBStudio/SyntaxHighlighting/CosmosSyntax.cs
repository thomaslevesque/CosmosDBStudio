using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace CosmosDBStudio.SyntaxHighlighting
{
    internal static class CosmosSyntax
    {
        public static void Init()
        {
            var assembly = typeof(CosmosSyntax).Assembly;
            var regex = new Regex(@"CosmosDBStudio\.SyntaxHighlighting\.(?<syntaxName>[a-zA-Z0-9-_]+).xshd");
            var resourceNames = assembly.GetManifestResourceNames();
            foreach (var resourceName in resourceNames)
            {
                var match = regex.Match(resourceName);
                if (!match.Success)
                    continue;

                var syntaxName = match.Groups["syntaxName"].Value;
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var reader = XmlReader.Create(stream);
                var definition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                HighlightingManager.Instance.RegisterHighlighting(syntaxName, Array.Empty<string>(), definition);
            }
        }
    }
}
