﻿using Microsoft.CodeAnalysis;
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
    public class AbstractClassChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = "TW2100";
        private static readonly LocalizableString _title = "Abstract classes should not have any method that has a body";
        private static readonly LocalizableString _messageFormat = "Abstract classes should not have any method that has a body";
        private const string _category = "Inheritance";

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, _category, DiagnosticSeverity.Error, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(_rule); } }

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.ClassDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {

            if (PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId)) return;

            var classDec = (ClassDeclarationSyntax) context.Node;

            var isAbstract = false;

            foreach (var item in classDec.Modifiers)
            {
                if(item.IsKind(SyntaxKind.AbstractKeyword))
                {
                    isAbstract = true;
                    break;
                }
            }

            if (isAbstract)
            {
                foreach (var member in classDec.Members)
                {
                    var isItSuitable = false;
                    foreach (var item in member.Modifiers)
                    {
                        if(item.IsKind(SyntaxKind.AbstractKeyword))
                        {
                            isItSuitable = true;
                            break;
                        }

                        if(item.IsKind(SyntaxKind.VirtualKeyword) && 
                            member.IsKind(SyntaxKind.MethodDeclaration))
                        {
                            var body = ((MethodDeclarationSyntax)member).Body;
                            if ( body == null || !body.Statements.Any())
                            {
                                isItSuitable = true;
                            }
                            break;
                        }
                    }

                    if(member.IsKind(SyntaxKind.MethodDeclaration))
                    {
                        var method = (MethodDeclarationSyntax)member;
                        if (!isItSuitable)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(_rule, method.Identifier.GetLocation()));
                        }
                    }
                }
            }

        }
    }
}
