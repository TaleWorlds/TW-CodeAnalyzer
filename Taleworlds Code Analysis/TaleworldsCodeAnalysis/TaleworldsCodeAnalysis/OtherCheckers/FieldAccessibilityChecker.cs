using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using TaleworldsCodeAnalysis.NameChecker;

namespace TaleworldsCodeAnalysis.OtherCheckers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FieldAccessibilityChecker : DiagnosticAnalyzer
    {
        public static string AccesabilityDiagnosticId => _accessibilityDiagnosticId;
        private const string _accessibilityDiagnosticId = "FieldAccessibilityChecker";
        private static readonly LocalizableString _accessibilityTitle = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _accessibilityMessageFormat = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _accessibilityDescription = new LocalizableResourceString(nameof(NameCheckerResources.FieldAccessibilityCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _accessibilityCategory = "Accessibility";

        private static readonly DiagnosticDescriptor _accessibilityRule = new DiagnosticDescriptor(_accessibilityDiagnosticId, _accessibilityTitle, _accessibilityMessageFormat, _accessibilityCategory, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _accessibilityDescription);



        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(_accessibilityRule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.FieldDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {

            if (BlackListedProjects.Instance.isBlackListedProjectFromCodePath(context.Node.SyntaxTree.FilePath)) return;
            WhiteListParser.Instance.ReadGlobalWhiteListPath(context.Node.SyntaxTree.FilePath);

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
                var diagnostic = Diagnostic.Create(_accessibilityRule, location, properties.ToImmutableDictionary(), nameString);
                context.ReportDiagnostic(diagnostic);
            }
        }

    }
}
