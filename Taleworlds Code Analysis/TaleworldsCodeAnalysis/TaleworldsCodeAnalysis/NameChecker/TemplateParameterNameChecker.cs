using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TaleworldsCodeAnalysis.NameChecker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TemplateParameterNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        public const string _diagnosticId = "TemplateParameterNameChecker";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.TemplateParameterNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.TemplateParameterNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.TemplateParameterNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _category = "Naming";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.TypeParameter);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            WhiteListParser.Instance.UpdateWhiteList(context.Options.AdditionalFiles);

            var parameter = (TypeParameterSyntax)context.Node;
            var parameterName = parameter.Identifier.Text;

            var properties = new Dictionary<string, string>
            {
                { "Name", parameterName },
            };

            if (!(parameterName.StartsWith("T") && NameCheckerLibrary.IsMatchingConvention(parameterName, ConventionType.TPascalCase)))
            {
                properties["NamingConvention"] = "TPascalCase";
                context.ReportDiagnostic(Diagnostic.Create(_rule, parameter.GetLocation(), properties.ToImmutableDictionary(), parameterName));
            }
            
        }
    }
}
