using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using TaleworldsCodeAnalysis.NameChecker;

namespace TaleworldsCodeAnalysis.OtherCheckers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FieldAccessibilityChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;
        private const string _diagnosticId = "TW2200";
        private static readonly LocalizableString _accessibilityTitle = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _accessibilityMessageFormat = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _accessibilityDescription = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _accessibilityCategory = "Accessibility";

        private static DiagnosticDescriptor _accessibilityRule = new DiagnosticDescriptor(_diagnosticId, _accessibilityTitle, _accessibilityMessageFormat, _accessibilityCategory, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _accessibilityDescription);



        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(_accessibilityRule); }
        }

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.FieldDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if (PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId)) return;

            var nameNode = (FieldDeclarationSyntax)context.Node;
            var nameString = nameNode.Declaration.Variables.First().Identifier.ToString();

            var accessibility = nameNode.Modifiers.First();
            var location = nameNode.Declaration.Variables.First().Identifier.GetLocation();

            if (nameNode.Parent.IsKind(SyntaxKind.EnumDeclaration))
            {
                return;
            }

            var properties = new Dictionary<string, string>
            {
                { "Name", nameString },
            };

            if (!accessibility.IsKind(SyntaxKind.PrivateKeyword))
            {
                var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _accessibilityRule.DefaultSeverity);
                _accessibilityRule = new DiagnosticDescriptor(_diagnosticId, _accessibilityTitle, _accessibilityMessageFormat, _accessibilityCategory, severity, isEnabledByDefault: true, description: _accessibilityDescription);
                var diagnostic = Diagnostic.Create(_accessibilityRule, location, properties.ToImmutableDictionary(), nameString);
                context.ReportDiagnostic(diagnostic);
            }
        }

    }
}
