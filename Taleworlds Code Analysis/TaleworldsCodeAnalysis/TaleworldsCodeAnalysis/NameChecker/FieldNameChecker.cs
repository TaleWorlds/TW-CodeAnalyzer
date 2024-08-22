using Microsoft.CodeAnalysis;
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
        private const string _nameDiagnosticId = "FieldNameChecker";
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
            context.RegisterSymbolAction(_analyzer, SymbolKind.Field);
        }

        private void _analyzer(SymbolAnalysisContext context)
        {
            if (BlackListedProjects.Instance.isBlackListedProjectFromCodePath(context.Symbol.Locations[0].ToString())) return;

            WhiteListParser.Instance.UpdateWhiteList(context.Options.AdditionalFiles);

            var field = (IFieldSymbol)context.Symbol;

            if(field.ContainingType.TypeKind==TypeKind.Enum)
            {
                return;
            }

            var properties = new Dictionary<string, string>
            {
                { "Name", field.Name },
            };

            if (field.DeclaredAccessibility == Accessibility.Private)
            {
                if (!UnderScoreCaseBehaviour.Instance.IsMatching(field.Name))
                {
                    properties["NamingConvention"] = "_uscoreCase";
                    var diagnostic = Diagnostic.Create(_nameRule, field.Locations[0], properties.ToImmutableDictionary(), field.Name, 
                        UnderScoreCaseBehaviour.Instance.FixThis(field.Name));
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

    }
}
