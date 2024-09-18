﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using TaleworldsCodeAnalysis.NameChecker;

namespace TaleworldsCodeAnalysis.OtherCheckers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MutebleFieldReturnChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;
        private const string _diagnosticId = "TW2204";
        private static readonly string _title = "MutableFieldReturnChecker Title";
        private static readonly string _messageFormat = "You should not return a 'List' that is a field. It can be modified outside.";
        private static readonly string _description = "Description";
        private const string _category = "Encapsulation";

        private static readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: _description);



        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(_rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_returnAnalyzer, SyntaxKind.ReturnStatement);
        }

        private void _returnAnalyzer(SyntaxNodeAnalysisContext context)
        {
            if (PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId)) return;
            var node = (ReturnStatementSyntax)context.Node;


            var includingMethod = (MethodDeclarationSyntax)node.AncestorsAndSelf().FirstOrDefault(elem => elem.IsKind(SyntaxKind.MethodDeclaration));
            var returnType = includingMethod.ReturnType;
            if (!(returnType is GenericNameSyntax)) return;
            var returnTypeIdentifier = ((GenericNameSyntax)returnType).Identifier;
            if (returnTypeIdentifier.ValueText != "List") return;

            var returnExpression = node.Expression;
            if (!(returnExpression is IdentifierNameSyntax)) return;
            var returnedIdentifierName = ((IdentifierNameSyntax)returnExpression).Identifier.ValueText;

            var includingClass = (ClassDeclarationSyntax)node.AncestorsAndSelf().FirstOrDefault(elem => elem.IsKind(SyntaxKind.ClassDeclaration));
            var classFields = includingClass.Members.Where(elem => elem.IsKind(SyntaxKind.FieldDeclaration)).Select(elem => (FieldDeclarationSyntax)elem);
            var returnedValueIsField = classFields.Any(elem => elem.Declaration.Variables.Any(variable => variable.Identifier.ValueText == returnedIdentifierName));
            
            
            if (returnedValueIsField)
            {
               
                var diagnosticLocation = node.GetLocation();
                context.ReportDiagnostic(Diagnostic.Create(_rule, diagnosticLocation));
            }

        }

    }
}
