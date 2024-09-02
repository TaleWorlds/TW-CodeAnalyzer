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
    public class ClassAccessibilityChecker : DiagnosticAnalyzer
    {
        public static string ModifierDiagnosticId => _modifierDiagnosticId;

        private const string _modifierDiagnosticId = "TW2001";

        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.ClassModifierCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.ClassModifierCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _modifierMessageFormat = new LocalizableResourceString(nameof(NameCheckerResources.ClassModifierCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));

        private const string _category = "Accessibility";

        private static readonly DiagnosticDescriptor _modifierRule = new DiagnosticDescriptor(_modifierDiagnosticId, _title, _modifierMessageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_modifierRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.ClassDeclaration);

        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            var nameNode = (ClassDeclarationSyntax)context.Node;
            var nameString = nameNode.Identifier.Text;
            var accessibility = nameNode.Modifiers.First();
            var location = nameNode.Identifier.GetLocation();

            if (PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context)) return;


            var properties = new Dictionary<string, string>
            {
                { "Name", nameString},
            };

            if (accessibility.IsKind(SyntaxKind.ProtectedKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(_modifierRule, location, properties.ToImmutableDictionary(), nameString));
            }
        }
    }
}