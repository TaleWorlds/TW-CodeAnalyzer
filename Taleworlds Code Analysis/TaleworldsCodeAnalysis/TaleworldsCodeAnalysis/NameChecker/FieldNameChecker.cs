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
    public class FieldNameChecker : DiagnosticAnalyzer
    {
        public static string NameDiagnosticId => _nameDiagnosticId;
        private const string _nameDiagnosticId = "TW2002";
        private static readonly LocalizableString _nameTitle = new LocalizableResourceString(nameof(NameCheckerResources.FieldNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _nameMessageFormat = new LocalizableResourceString(nameof(NameCheckerResources.FieldNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _nameDescription = new LocalizableResourceString(nameof(NameCheckerResources.FieldNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _namingCategory = "Naming";

        private static readonly DiagnosticDescriptor _nameRule = new DiagnosticDescriptor(_nameDiagnosticId, _nameTitle, _nameMessageFormat, _namingCategory, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _nameDescription);



        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(_nameRule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.FieldDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            var nameNode = (FieldDeclarationSyntax)context.Node;
            var nameString = nameNode.Declaration.Variables.First().Identifier.ToString();
            var accessibility = nameNode.Modifiers.First();
            var location = nameNode.Declaration.Variables.First().Identifier.GetLocation();
            if (PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context)) return;

            if (nameNode.Parent.IsKind(SyntaxKind.EnumDeclaration))
            {
                return;
            }

            var properties = new Dictionary<string, string>
            {
                { "Name", nameString },
            };

            if (accessibility.IsKind(SyntaxKind.PrivateKeyword))
            {
                if (!UnderScoreCaseBehaviour.Instance.IsMatching(nameString))
                {
                    properties["NamingConvention"] = "_uscoreCase";
                    var diagnostic = Diagnostic.Create(_nameRule, location, properties.ToImmutableDictionary(), nameString, 
                        UnderScoreCaseBehaviour.Instance.FixThis(nameString));
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

    }
}
