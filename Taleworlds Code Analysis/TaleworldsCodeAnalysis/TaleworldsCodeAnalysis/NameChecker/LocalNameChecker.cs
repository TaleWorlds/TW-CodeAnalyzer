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
    [TaleworldsAnalyzer("Locan Name Checker", _diagnosticId, title: "Naming Checker")]
    public class LocalNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = nameof(DiagnosticIDs.TW2004);
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.LocalNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.LocalNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const DiagnosticCategories _category = DiagnosticCategories.Naming;

        private static  DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, nameof(_category), DiagnosticSeverity.Error, isEnabledByDefault: true);


        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.LocalDeclarationStatement);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if (!PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId))
            {
                var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;
                var localName = localDeclaration.Declaration.Variables.Single().Identifier.ToString();
                var location = localDeclaration.Declaration.Variables.Single().Identifier.GetLocation();

                var properties = new Dictionary<string, string>
                {
                    { "Name", localName },
                };

                if (!CamelCaseBehaviour.Instance.IsMatching(localName))
                {
                    properties["NamingConvention"] = nameof(ConventionType.CamelCase);
                    var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _rule.DefaultSeverity);
                    _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, nameof(_category), severity, isEnabledByDefault: true);
                    context.ReportDiagnostic(Diagnostic.Create(_rule, location, properties.ToImmutableDictionary(), localName, CamelCaseBehaviour.Instance.FixThis(localName)));
                }
            } 
        }
    }
}
