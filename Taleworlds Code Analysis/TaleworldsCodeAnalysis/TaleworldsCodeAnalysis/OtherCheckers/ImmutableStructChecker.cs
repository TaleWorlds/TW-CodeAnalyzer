﻿using System;
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
        public string DiagnosticId => _diagnosticId;
        private const string _diagnosticId = "TW2205";
        private static readonly LocalizableString _title = "";
        private static readonly LocalizableString _messageFormat = "Structs that has only one field should be immutable (Place readonly modifier on the field)";
        private const string _category = "Immutable Structs";

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Warning, true);

        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.StructDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            var declarationNode = (StructDeclarationSyntax)context.Node;
            var members = declarationNode.Members;

            int fieldCount = 0;
            FieldDeclarationSyntax field = null; ;

            foreach (var member in members)
            {
                if (fieldCount > 1)
                {
                    break;
                }
                if (member.IsKind(SyntaxKind.FieldDeclaration))
                {
                    fieldCount++;
                    field = (FieldDeclarationSyntax)member;
                }
            }

            if (fieldCount==1)
            {
                var modifiers = field.Modifiers;
                var readOnly = false;
                foreach (var modifier in modifiers)
                {
                    //Immutable check
                    if(readOnly)
                    {
                        return;
                    }
                    if (modifier.IsKind(SyntaxKind.ReadOnlyKeyword))
                    {
                        readOnly = true;
                    }
                }

                if(!readOnly)
                {
                    context.ReportDiagnostic(Diagnostic.Create(_rule, declarationNode.Identifier.GetLocation()));
                }
            }


        }
    }

    
}
