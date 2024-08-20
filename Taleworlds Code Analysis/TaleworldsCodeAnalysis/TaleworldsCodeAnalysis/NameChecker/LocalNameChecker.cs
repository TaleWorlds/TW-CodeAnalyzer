using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TaleworldsCodeAnalysis.NameChecker.Conventions;

namespace TaleworldsCodeAnalysis.NameChecker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LocalNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = "LocalNameChecker";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.LocalNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.LocalNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.LocalNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _category = "Naming";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.LocalDeclarationStatement);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            WhiteListParser.Instance.UpdateWhiteList(context.Options.AdditionalFiles);

            var local = (LocalDeclarationStatementSyntax) context.Node;
            var localDeclaration = local.Declaration.Variables.Single();
            ISymbol localName = context.SemanticModel.GetDeclaredSymbol(localDeclaration, context.CancellationToken);

            var properties = new Dictionary<string, string>
            {
                { "Name", localName.Name },
            };

            if (!CamelCaseBehaviour.Instance.IsMatching(localName.Name))
            {
                properties["NamingConvention"] = "camelCase";
                context.ReportDiagnostic(Diagnostic.Create(_rule, localName.Locations[0], properties.ToImmutableDictionary(), localName));
            }
            
        }
    }
}
