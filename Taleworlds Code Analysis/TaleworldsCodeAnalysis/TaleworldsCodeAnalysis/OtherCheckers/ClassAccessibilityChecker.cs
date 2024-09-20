using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using TaleworldsCodeAnalysis.NameChecker.Conventions;

namespace TaleworldsCodeAnalysis.NameChecker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [TaleworldsAnalyzer("Class Accessibility Checker", _diagnosticId, title: "Other Checkers")]
    public class ClassAccessibilityChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = nameof(DiagnosticIDs.TW2001);
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.ClassModifierCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _modifierMessageFormat = new LocalizableResourceString(nameof(NameCheckerResources.ClassModifierCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));

        private const string _category = nameof(DiagnosticCategories.Accessibility);

        private static  DiagnosticDescriptor _modifierRule = new DiagnosticDescriptor(_diagnosticId, _title, _modifierMessageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true);

        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_modifierRule);

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.ClassDeclaration);

        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if (!PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId))
            {
                var nameNode = (ClassDeclarationSyntax)context.Node;
                var nameString = nameNode.Identifier.Text;
                var accessibility = nameNode.Modifiers.First();
                var location = nameNode.Identifier.GetLocation();



                var properties = new Dictionary<string, string>
                {
                    { "Name", nameString},
                };

                if (accessibility.IsKind(SyntaxKind.ProtectedKeyword))
                {
                    var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _modifierRule.DefaultSeverity);
                    _modifierRule = new DiagnosticDescriptor(_diagnosticId, _title, _modifierMessageFormat, _category, severity, isEnabledByDefault: true);
                    context.ReportDiagnostic(Diagnostic.Create(_modifierRule, location, properties.ToImmutableDictionary(), nameString));
                }
            }
            
        }
    }
}