using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using TaleworldsCodeAnalysis.NameChecker.Conventions;

namespace TaleworldsCodeAnalysis.NameChecker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [TaleworldsAnalyzer("Template Parameter Name Checker", _diagnosticId, title: "Naming Checker")]
    public class TemplateParameterNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = nameof(DiagnosticIDs.TW2008);
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.TemplateParameterNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.TemplateParameterNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _category = nameof(DiagnosticCategories.Naming);

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true);


        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.TypeParameter);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if (!PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId))
            {
                var parameter = (TypeParameterSyntax)context.Node;
                var parameterName = parameter.Identifier.Text;
                var location = parameter.GetLocation();

                var properties = new Dictionary<string, string>
                {
                    { "Name", parameterName },
                };

                if (!TPascalCaseBehaviour.Instance.IsMatching(parameterName))
                {
                    properties["NamingConvention"] = "TPascalCase";
                    var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _rule.DefaultSeverity);
                    _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, severity, isEnabledByDefault: true);
                    context.ReportDiagnostic(Diagnostic.Create(_rule, location, properties.ToImmutableDictionary(), parameterName,
                        TPascalCaseBehaviour.Instance.FixThis(parameterName)));
                }
            } 
        }
    }
}
