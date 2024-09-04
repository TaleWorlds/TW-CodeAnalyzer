using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TaleworldsCodeAnalysis.OtherCheckers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ImmutableStructChecker : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ImmutableStructChecker";
        internal static readonly LocalizableString Title = "ImmutableStructChecker Title";
        internal static readonly LocalizableString MessageFormat = "ImmutableStructChecker '{0}'";
        internal const string Category = "ImmutableStructChecker Category";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            //context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.StructDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            var declarationNode = (StructDeclarationSyntax)context.Node;
            var members = declarationNode.Members;

            int fieldCount = 0;

            foreach (var member in members)
            {
                if (fieldCount > 1)
                {
                    break;
                }
                if (member.IsKind(SyntaxKind.FieldDeclaration))
                {
                    fieldCount++;
                }
            }

            if (fieldCount==1)
            {
                var modifiers = declarationNode.Modifiers;
                var immutableFound = false;
                foreach (var modifier in modifiers)
                {
                    //Immutable check
                }
            }


        }
    }

    
}
