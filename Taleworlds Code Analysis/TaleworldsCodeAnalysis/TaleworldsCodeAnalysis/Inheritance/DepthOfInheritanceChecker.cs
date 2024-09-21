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
    [TaleworldsAnalyzer("Depth of Inheritance Checker", _diagnosticId,title:"Inheritance Checker")]
    public class DepthOfInheritanceChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = nameof(DiagnosticIDs.TW2101);
        private static readonly LocalizableString _title = 
            new LocalizableResourceString(nameof(InheritanceResources.DepthOfInheritanceTitle), InheritanceResources.ResourceManager, typeof(InheritanceResources));
        private static readonly LocalizableString _messageFormat = 
            new LocalizableResourceString(nameof(InheritanceResources.DepthOfInheritanceMessageFormat),InheritanceResources.ResourceManager,typeof(InheritanceResources));
        private const DiagnosticCategories _category = DiagnosticCategories.Inheritance;
        private const int _maxAllowedDepth = 2;

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, nameof(_category), DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule); 

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.ClassDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if (!PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId))
            {
                var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
                var semanticModel = context.SemanticModel;
                var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax);

                var currentBaseType = classSymbol.BaseType;
                int depth = 1; // 1 is minimum depth of inheritance that a class can have.
                while (currentBaseType != null && currentBaseType.SpecialType != SpecialType.System_Object)
                {
                    depth++;
                    currentBaseType = currentBaseType.BaseType;
                }

                if (depth > _maxAllowedDepth)
                {
                    context.ReportDiagnostic(Diagnostic.Create(_rule, classDeclarationSyntax.Identifier.GetLocation()));
                }
            }   
        }
    }
}
