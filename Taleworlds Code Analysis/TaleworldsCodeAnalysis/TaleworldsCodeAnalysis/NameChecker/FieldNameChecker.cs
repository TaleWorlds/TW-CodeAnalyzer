﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace TaleworldsCodeAnalysis.NameChecker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FieldNameChecker : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FieldNameChecker";
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.FieldNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.FieldNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _description = new LocalizableResourceString(nameof(NameCheckerResources.FieldNameCheckerDescription), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const string _category = "Naming";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(DiagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(_rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(_analyzeField, SymbolKind.Field);
        }

        private void _analyzeField(SymbolAnalysisContext context)
        {
            var field = (IFieldSymbol)context.Symbol;

            if (field.DeclaredAccessibility == Accessibility.Private)
            {
                _checkPrivateField(field, context);
            }
            else
            {
                _createDiagnostic(field, context);
            }
        }

        private void _checkPrivateField(ISymbol field, SymbolAnalysisContext context)
        {
            if (!NameCheckerLibrary.IsUnderScoreCase(field.Name))
            {
                _createDiagnostic(field, context);
            }
        }

        private void _createDiagnostic(ISymbol field, SymbolAnalysisContext context)
        {
            var diagnostic = Diagnostic.Create(_rule, field.Locations[0], field.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
