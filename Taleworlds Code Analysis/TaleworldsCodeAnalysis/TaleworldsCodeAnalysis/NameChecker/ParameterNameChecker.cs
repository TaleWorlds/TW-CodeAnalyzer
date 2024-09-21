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
    [TaleworldsAnalyzer("Parameter Name Checker", _diagnosticId, title: "Naming Checker")]
    public class ParameterNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = nameof(DiagnosticIDs.TW2006);
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.TemplateParameterNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.ParameterNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const DiagnosticCategories _category = DiagnosticCategories.Naming;

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, nameof(_category), DiagnosticSeverity.Error, isEnabledByDefault: true);


        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.Parameter);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            var nameNode = (ParameterSyntax)context.Node;
            var nameString = nameNode.Identifier.ToString();
            var location = nameNode.Identifier.GetLocation();
            if (!PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId))
            {
                var properties = new Dictionary<string, string>
                {
                    { "Name", nameString },
                };

                if (!CamelCaseBehaviour.Instance.IsMatching(nameString))
                {
                    properties["NamingConvention"] = nameof(ConventionType.CamelCase);
                    var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _rule.DefaultSeverity);
                    _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, nameof(_category), severity, isEnabledByDefault: true);
                    context.ReportDiagnostic(Diagnostic.Create(_rule, location, properties.ToImmutableDictionary(), nameString,
                        CamelCaseBehaviour.Instance.FixThis(nameString)));
                }
            }
        }
    }
}
