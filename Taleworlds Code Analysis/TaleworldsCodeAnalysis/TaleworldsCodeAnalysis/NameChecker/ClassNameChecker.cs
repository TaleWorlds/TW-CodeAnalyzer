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
    [TaleworldsAnalyzer("Class Name Checker", _diagnosticId, title: "Naming Checker")]
    public class ClassNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId =>_diagnosticId;

        private const string _diagnosticId = nameof(DiagnosticIDs.TW2000);

        private static readonly LocalizableString _title = 
            new LocalizableResourceString(nameof(NameCheckerResources.ClassNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = 
            new LocalizableResourceString(nameof(NameCheckerResources.ClassNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        
        private const string _category = nameof(DiagnosticCategories.Naming);

        private static DiagnosticDescriptor _nameRule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error,true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_nameRule);

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer,SyntaxKind.ClassDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if (!PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId))
            {
                var nameNode = (ClassDeclarationSyntax)context.Node;
                var nameString = nameNode.Identifier.Text;
                var accessibility = nameNode.Modifiers.First();
                var location = nameNode.Identifier.GetLocation();
                var filePath = context.Node.GetLocation().SourceTree.FilePath;
                var properties = new Dictionary<string, string>
                {
                    { "Name", nameString},
                };

                if (accessibility.IsKind(SyntaxKind.PrivateKeyword) ||
                    accessibility.IsKind(SyntaxKind.InternalKeyword))
                {
                    if (!UnderScoreCaseBehaviour.Instance.IsMatching(nameString))
                    {
                        context.ReportDiagnostic(_createDiagnostic(ConventionType.UnderScoreCase, properties, location, filePath));
                    }
                }
                else if (accessibility.IsKind(SyntaxKind.PublicKeyword))
                {
                    if (!PascalCaseBehaviour.Instance.IsMatching(nameString))
                    {
                        context.ReportDiagnostic(_createDiagnostic(ConventionType.PascalCase, properties, location, filePath));
                    }
                }
            }
        }

        private Diagnostic _createDiagnostic(ConventionType conventionType, Dictionary<string, string> properties, Location location, string filePath)
        {
            properties["NamingConvention"] = nameof(conventionType);
            var nameString = properties["Name"];
            var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, filePath, _nameRule.DefaultSeverity);
            _nameRule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, severity, isEnabledByDefault: true);
            var stringAfterFix = "";
            var diagnostic = default(Diagnostic);
            switch (conventionType) 
            {
                case ConventionType.UnderScoreCase:
                    stringAfterFix = UnderScoreCaseBehaviour.Instance.FixThis(nameString);
                    break;
                case ConventionType.PascalCase:
                    stringAfterFix = PascalCaseBehaviour.Instance.FixThis(nameString);
                    break;
            }
            diagnostic = Diagnostic.Create(_nameRule, location, properties.ToImmutableDictionary(), nameString, stringAfterFix);
            return diagnostic;
        }
    }
}