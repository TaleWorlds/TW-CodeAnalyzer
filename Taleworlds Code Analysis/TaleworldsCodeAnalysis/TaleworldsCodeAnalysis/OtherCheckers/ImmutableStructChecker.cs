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
        public string DiagnosticId => _diagnosticId;
        private const string _diagnosticId = nameof(DiagnosticIDs.TW2205);
        private static readonly LocalizableString _title = 
            new LocalizableResourceString(nameof(OtherCheckerResource.ImmutableStructCheckerTitle),OtherCheckerResource.ResourceManager,typeof(OtherCheckerResource));
        private static readonly LocalizableString _messageFormat =
            new LocalizableResourceString(nameof(OtherCheckerResource.ImmutableStructCheckerMessage), OtherCheckerResource.ResourceManager, typeof(OtherCheckerResource));
        private const DiagnosticCategories _category = DiagnosticCategories.Immutable;

        private static DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, nameof(_category), DiagnosticSeverity.Warning, true);

        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.StructDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if(!PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId))
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

                if (fieldCount == 1)
                {
                    var modifiers = field.Modifiers;
                    var readOnly = false;
                    foreach (var modifier in modifiers)
                    {
                        //Immutable check
                        if (readOnly)
                        {
                            return;
                        }
                        if (modifier.IsKind(SyntaxKind.ReadOnlyKeyword))
                        {
                            readOnly = true;
                        }
                    }

                    if (!readOnly)
                    {
                        var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, context.Node.GetLocation().SourceTree.FilePath, _rule.DefaultSeverity);
                        _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, nameof(_category), severity, isEnabledByDefault: true);
                        context.ReportDiagnostic(Diagnostic.Create(_rule, declarationNode.Identifier.GetLocation()));
                    }
                }

            }


        }
    }

    
}
