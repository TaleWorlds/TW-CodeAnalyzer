﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace TaleworldsCodeAnalysis.NameChecker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FieldNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;
        private const string _diagnosticId = "FieldNameChecker";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.FieldNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.FieldNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.FieldNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _category = "Naming";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(_rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(_analyzer, SymbolKind.Field);
        }

        private void _analyzer(SymbolAnalysisContext context)
        {
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
                if (!NameCheckerLibrary.IsMatchingConvention(field.Name, ConventionType._uscoreCase))
                {
                    properties["NamingConvention"] = "_uscoreCase";
                    var diagnostic = Diagnostic.Create(_rule, field.Locations[0], properties.ToImmutableDictionary(), field.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
            else
            {
                var diagnostic = Diagnostic.Create(_rule, field.Locations[0], properties.ToImmutableDictionary(), field.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

    }
}
