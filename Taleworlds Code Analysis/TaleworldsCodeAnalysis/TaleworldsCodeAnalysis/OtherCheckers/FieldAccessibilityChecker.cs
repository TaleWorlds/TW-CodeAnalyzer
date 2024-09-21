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
    [TaleworldsAnalyzer("Field Accessibility Checker", _diagnosticId, title: "Other Checkers")]
    public class FieldAccessibilityChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;
        private const string _diagnosticId = nameof(DiagnosticIDs.TW2200);
        private static readonly LocalizableString _accessibilityTitle = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _accessibilityMessageFormat = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const DiagnosticCategories _category = DiagnosticCategories.Accessibility;

        private static DiagnosticDescriptor _accessibilityRule = new DiagnosticDescriptor(_diagnosticId, _accessibilityTitle, _accessibilityMessageFormat, nameof(_category), DiagnosticSeverity.Error, isEnabledByDefault: true);

        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_accessibilityRule);

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.FieldDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if (!PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId))
            {
                var nameNode = (FieldDeclarationSyntax)context.Node;
                var nameString = nameNode.Declaration.Variables.First().Identifier.ToString();

                var accessibility = nameNode.Modifiers.First();
                var location = nameNode.Declaration.Variables.First().Identifier.GetLocation();

                if (!nameNode.Parent.IsKind(SyntaxKind.EnumDeclaration))
                {
                    var properties = new Dictionary<string, string>
                    {
                        { "Name", nameString },
                    };

                    if (!accessibility.IsKind(SyntaxKind.PrivateKeyword))
                    {
                        var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _accessibilityRule.DefaultSeverity);
                        _accessibilityRule = new DiagnosticDescriptor(_diagnosticId, _accessibilityTitle, _accessibilityMessageFormat, nameof(_category), severity, isEnabledByDefault: true);
                        var diagnostic = Diagnostic.Create(_accessibilityRule, location, properties.ToImmutableDictionary(), nameString);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
