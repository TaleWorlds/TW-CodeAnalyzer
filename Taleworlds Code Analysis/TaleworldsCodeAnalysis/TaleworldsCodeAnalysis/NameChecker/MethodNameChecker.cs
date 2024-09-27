using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using TaleworldsCodeAnalysis.NameChecker.Conventions;

namespace TaleworldsCodeAnalysis.NameChecker
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [TaleworldsAnalyzer("Method Name Checker", _diagnosticId, title: "Naming Checker")]
    public class MethodNameChecker : DiagnosticAnalyzer
    {
        public static string DiagnosticId => _diagnosticId;

        private const string _diagnosticId = nameof(DiagnosticIDs.TW2005);
        private static readonly LocalizableString _title = new LocalizableResourceString(nameof(NameCheckerResources.MethodNameCheckerTitle), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private static readonly LocalizableString _messageFormat = new LocalizableResourceString(nameof(NameCheckerResources.MethodNameCheckerMessageFormat), NameCheckerResources.ResourceManager, typeof(NameCheckerResources));
        private const DiagnosticCategories _category = DiagnosticCategories.Naming;

        private static  DiagnosticDescriptor _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, nameof(_category), DiagnosticSeverity.Error, isEnabledByDefault: true);


        public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

        public sealed override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(_analyzer, SyntaxKind.MethodDeclaration);
        }

        private void _analyzer(SyntaxNodeAnalysisContext context)
        {
            if (!PreAnalyzerConditions.Instance.IsNotAllowedToAnalyze(context, DiagnosticId))
            {
                var nameNode = (MethodDeclarationSyntax)context.Node;
                var nameString = nameNode.Identifier.ToString();
                var accessibility = nameNode.Modifiers.First();
                var location = nameNode.Identifier.GetLocation();
                var filePath = context.Node.GetLocation().SourceTree.FilePath;

                if (!nameNode.IsKind(SyntaxKind.GetAccessorDeclaration) &&
                    !nameNode.IsKind(SyntaxKind.SetAccessorDeclaration) &&
                    !nameNode.IsKind(SyntaxKind.ConstructorDeclaration))
                {
                    var properties = new Dictionary<string, string>
                    {
                        { "Name", nameString },
                    };

                    Diagnostic diagnostic = null;

                    if (accessibility.IsKind(SyntaxKind.PrivateKeyword) ||
                        accessibility.IsKind(SyntaxKind.InternalKeyword))
                    {
                        if (!UnderScoreCaseBehaviour.Instance.IsMatching(nameString))
                        {
                            diagnostic = _createDiagnostic(ConventionType.UnderScoreCase, properties, location, filePath);
                        }
                    }
                    else
                    {
                        if (!PascalCaseBehaviour.Instance.IsMatching(nameString))
                        {
                            diagnostic =_createDiagnostic(ConventionType.PascalCase, properties, location, filePath);
                        }
                    }
                    if (diagnostic != null)
                    {
                        context.ReportDiagnostic(diagnostic);
                    }  
                } 
            } 
        }
        private Diagnostic _createDiagnostic(ConventionType conventionType, Dictionary<string, string> properties, Location location, string filePath)
        {
            switch(conventionType)
            {
                case ConventionType.UnderScoreCase:
                    properties["NamingConvention"] = nameof(ConventionType.UnderScoreCase);
                    break;
                case ConventionType.PascalCase:
                    properties["NamingConvention"] = nameof(ConventionType.PascalCase);
                    break;
                case ConventionType.CamelCase:
                    properties["NamingConvention"] = nameof(ConventionType.CamelCase);
                    break;
                case ConventionType.IPascalCase:
                    properties["NamingConvention"] = nameof(ConventionType.IPascalCase);
                    break;
                case ConventionType.TPascalCase:
                    properties["NamingConvention"] = nameof(ConventionType.TPascalCase);
                    break;
            }
            
            var nameString = properties["Name"];
            var severity = SettingsChecker.Instance.GetDiagnosticSeverity(_diagnosticId, filePath, _rule.DefaultSeverity);
            _rule = new DiagnosticDescriptor(_diagnosticId, _title, _messageFormat, nameof(_category), severity, isEnabledByDefault: true);
            var stringAfterFix = "";
            var diagnostic = default(Diagnostic);
            switch (conventionType)
            {
                case ConventionType.UnderScoreCase:
                    stringAfterFix = UnderScoreCaseBehaviour.Instance.FixThis(nameString);
                    break;
                case ConventionType.PascalCase:
                    stringAfterFix = PascalCaseBehaviour.Instance.FixThis(nameString);
                    break;
            }
            diagnostic = Diagnostic.Create(_rule, location, properties.ToImmutableDictionary(), nameString, stringAfterFix);
            return diagnostic;
        }
    }
}
