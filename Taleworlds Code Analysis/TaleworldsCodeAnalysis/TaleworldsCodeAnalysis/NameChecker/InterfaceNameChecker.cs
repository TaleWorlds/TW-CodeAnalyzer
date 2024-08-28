using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using TaleworldsCodeAnalysis.NameChecker.Conventions;

namespace TaleworldsCodeAnalysis.NameChecker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InterfaceNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = "InterfaceNameChecker";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.InterfaceNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.InterfaceNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.InterfaceNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _category = "Naming";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.InterfaceDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if (BlackListedProjects.Instance.isBlackListedProjectFromCodePath(context.Symbol.Locations[0].SourceTree.FilePath)) return;

            var nameNode = (InterfaceDeclarationSyntax)context.Node;
            var nameString = nameNode.Identifier.Text;
            var accessibility = nameNode.Modifiers.First();
            var location = nameNode.Identifier.GetLocation();

            WhiteListParser.Instance.ReadGlobalWhiteListPath(location.SourceTree.FilePath);

            var properties = new Dictionary<string, string>
            {
                { "Name", nameString },
            };

            if(!IpascalCaseBehaviour.Instance.IsMatching(nameString))
            {
                properties["NamingConvention"] = "IPascalCase";
                context.ReportDiagnostic(Diagnostic.Create(_rule, location, properties.ToImmutableDictionary(), nameString,
                    IpascalCaseBehaviour.Instance.FixThis(nameString)));
            }

        }
    }
}
