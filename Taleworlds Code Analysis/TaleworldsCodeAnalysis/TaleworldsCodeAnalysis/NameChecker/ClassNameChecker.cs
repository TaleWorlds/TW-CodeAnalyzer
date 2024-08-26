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
    public class ClassNameChecker : DiagnosticAnalyzer
    {
        public static string NameDiagnosticId =>_nameDiagnosticId;
        public static string ModifierDiagnosticId=>_modifierDiagnosticId;

        private const string _nameDiagnosticId = "ClassNameChecker";
        private const string _modifierDiagnosticId = "ClassModifierChecker";

        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.ClassNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.ClassNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.ClassNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _modifierMessageFormat = new LocalizableResourceString(nameof(NameCheckerResources.ClassModifierCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));

        private const string _category = "Naming";

        private static readonly DiagnosticDescriptor _nameRule = new DiagnosticDescriptor(_nameDiagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);
        private static readonly DiagnosticDescriptor _modifierRule = new DiagnosticDescriptor(_modifierDiagnosticId, _title, _modifierMessageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_nameRule, _modifierRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer,SyntaxKind.ClassDeclaration);
            
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            var nameNode = (ClassDeclarationSyntax) context.Node;
            var nameString = nameNode.Identifier.Text;
            var accessibility = nameNode.Modifiers.First();
            var location = nameNode.Identifier.GetLocation();

            WhiteListParser.Instance.ReadGlobalWhiteListPath(location.SourceTree.FilePath);
            

            var properties = new Dictionary<string, string>
            {
                { "Name", nameString},
            };

            if (accessibility.IsKind(SyntaxKind.PrivateKeyword) ||
                accessibility.IsKind(SyntaxKind.InternalKeyword))
            {
                if (!UnderScoreCaseBehaviour.Instance.IsMatching(nameString))
                {
                    properties["NamingConvention"] = "_uscoreCase";
                    context.ReportDiagnostic(Diagnostic.Create(_nameRule, location, properties.ToImmutableDictionary(), nameString,
                        UnderScoreCaseBehaviour.Instance.FixThis(nameString)));
                }
            }
            else if (accessibility.IsKind(SyntaxKind.PublicKeyword))
            {
                if (!PascalCaseBehaviour.Instance.IsMatching(nameString))
                {
                    properties["NamingConvention"] = "PascalCase";
                    context.ReportDiagnostic(Diagnostic.Create(_nameRule, location, properties.ToImmutableDictionary(), nameString,
                        PascalCaseBehaviour.Instance.FixThis(nameString)));
                }
            }
            else
            {
                context.ReportDiagnostic(Diagnostic.Create(_modifierRule, location, nameString));
            }
        }
    }
}