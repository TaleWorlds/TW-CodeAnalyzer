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
    [TaleworldsAnalyzer("Field Name Checker", _diagnosticId, title: "Naming Checker")]
    public class FieldNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;
        private const string _diagnosticId = nameof(DiagnosticIDs.TW2002);
        private static readonly LocalizableString _nameTitle = new LocalizableResourceString(nameof(NameCheckerResources.FieldNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _nameMessageFormat = new LocalizableResourceString(nameof(NameCheckerResources.FieldNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const DiagnosticCategories _category = DiagnosticCategories.Naming;

        private static DiagnosticDescriptor _nameRule = new DiagnosticDescriptor(_diagnosticId, _nameTitle, _nameMessageFormat, nameof(_category), DiagnosticSeverity.Error, isEnabledByDefault: true);

        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get 
            { 
                return ImmutableArray.Create(_nameRule); 
            }
        }

        public sealed override void Initialize(AnalysisContext context)
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
            if (!PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId))
            {
                var properties = new Dictionary<string, string>
                {
                    { "Name", nameString },
                };

                if (nameNode.Parent.IsKind(SyntaxKind.EnumDeclaration))
                {
                    if (!PascalCaseBehaviour.Instance.IsMatching(nameString)) // Make this more reusable
                    {
                        var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _nameRule.DefaultSeverity);
                        _nameRule = new DiagnosticDescriptor(_diagnosticId, _nameTitle, _nameMessageFormat, nameof(_category), severity, isEnabledByDefault: true);
                        properties["NamingConvention"] = nameof(ConventionType.PascalCase);
                        var diagnostic = Diagnostic.Create(_nameRule, location, properties.ToImmutableDictionary(), nameString,
                            PascalCaseBehaviour.Instance.FixThis(nameString));
                        context.ReportDiagnostic(diagnostic);
                    }
                }
                else if (accessibility.IsKind(SyntaxKind.PrivateKeyword))
                {
                    if (!UnderScoreCaseBehaviour.Instance.IsMatching(nameString))
                    {
                        var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _nameRule.DefaultSeverity);
                        _nameRule = new DiagnosticDescriptor(_diagnosticId, _nameTitle, _nameMessageFormat, nameof(_category), severity, isEnabledByDefault: true);
                        properties["NamingConvention"] = nameof(ConventionType.UnderScoreCase);
                        var diagnostic = Diagnostic.Create(_nameRule, location, properties.ToImmutableDictionary(), nameString,
                            UnderScoreCaseBehaviour.Instance.FixThis(nameString));
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

    }
}
