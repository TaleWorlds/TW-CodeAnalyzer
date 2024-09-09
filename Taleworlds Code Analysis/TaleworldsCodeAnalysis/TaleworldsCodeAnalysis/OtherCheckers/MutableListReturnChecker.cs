using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using TaleworldsCodeAnalysis.NameChecker;

namespace TaleworldsCodeAnalysis.OtherCheckers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MutebleAccessibilityChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;
        private const string _diagnosticId = "TW2201";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _accessibilityCategory = "Encapsulation";

        private static readonly DiagnosticDescriptor _accessibilityRule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _accessibilityCategory, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);



        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(_accessibilityRule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.MethodDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {

            if (PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId)) return;

            var node = (MethodDeclarationSyntax)context.Node;
            var returnType = node.ReturnType;
            if (!(returnType is GenericNameSyntax)) return;
            var identifier = ((GenericNameSyntax)returnType).Identifier;

            if (identifier.ValueText != "List") return;

            var statements = node.Body.Statements;

            var a = 1;
           

            
        }

    }
}
