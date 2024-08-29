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
    public class ParameterNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = "ParameterNameChecker";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.ParameterNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.ParameterNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.ParameterNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _category = "Naming";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context)
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
            if (PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context)) return;


            var properties = new Dictionary<string, string>
            {
                { "Name", nameString },
            };

            if (!CamelCaseBehaviour.Instance.IsMatching(nameString))
            {
                properties["NamingConvention"] = "camelCase";
                context.ReportDiagnostic(Diagnostic.Create(_rule, location, properties.ToImmutableDictionary(),nameString,
                    CamelCaseBehaviour.Instance.FixThis(nameString)));
            }
            
        }
    }
}
