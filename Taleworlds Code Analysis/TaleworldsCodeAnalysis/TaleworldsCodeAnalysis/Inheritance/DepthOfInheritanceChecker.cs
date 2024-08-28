using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace TaleworldsCodeAnalysis.Inheritance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DepthOfInheritanceChecker : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "TW2102";
        internal static readonly LocalizableString Title = "Depth of inheritance should be 2 at maximum.";
        internal static readonly LocalizableString MessageFormat = "Depth of inheritance should be 2 at maximum.";
        internal const string Category = "Inheritance";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.ClassDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
            var semanticModel = context.SemanticModel;
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);

            var currentBaseType = classSymbol.BaseType;
            int depth = 0;
            while (currentBaseType != null && currentBaseType.SpecialType!=SpecialType.System_Object)
            {
                depth++;
                currentBaseType = currentBaseType.BaseType;
            }

            if (depth>1)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, classDeclarationSyntax.Identifier.GetLocation()));
            }


        }
    }
}
