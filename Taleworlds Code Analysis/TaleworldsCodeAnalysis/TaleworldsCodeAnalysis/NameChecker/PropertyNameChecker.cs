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
    [TaleworldsAnalyzer("Property Name Checker", _diagnosticId, title: "Naming Checker")]
    public class PropertyNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId =>_diagnosticId;

        private const string _diagnosticId = nameof(DiagnosticIDs.TW2007);
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.PropertyNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.PropertyNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _category = nameof(DiagnosticCategories.Naming);

        private static  DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true);


        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.PropertyDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            var nameNode = (PropertyDeclarationSyntax)context.Node;
            var nameString = nameNode.Identifier.ToString();
            var accessibility = nameNode.Modifiers.First();
            var location = nameNode.Identifier.GetLocation();

            if (!PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId))
            {
                var properties = new Dictionary<string, string>
                {
                    { "Name", nameString },
                };

                if (accessibility.IsKind(SyntaxKind.PrivateKeyword) ||
                     accessibility.IsKind(SyntaxKind.InternalKeyword))
                {
                    if (!UnderScoreCaseBehaviour.Instance.IsMatching(nameString))
                    {
                        properties["NamingConvention"] = nameof(ConventionType.UnderScoreCase);
                        var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _rule.DefaultSeverity);
                        _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, severity, isEnabledByDefault: true);
                        context.ReportDiagnostic(Diagnostic.Create(_rule, location, properties.ToImmutableDictionary(), nameString,
                            UnderScoreCaseBehaviour.Instance.FixThis(nameString)));
                    }
                }
                else
                {
                    if (!PascalCaseBehaviour.Instance.IsMatching(nameString))
                    {
                        properties["NamingConvention"] = nameof(ConventionType.PascalCase);
                        var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _rule.DefaultSeverity);
                        _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, severity, isEnabledByDefault: true);
                        context.ReportDiagnostic(Diagnostic.Create(_rule, location, properties.ToImmutableDictionary(), nameString,
                            PascalCaseBehaviour.Instance.FixThis(nameString)));
                    }
                }
            }
            
        }
    }
}
