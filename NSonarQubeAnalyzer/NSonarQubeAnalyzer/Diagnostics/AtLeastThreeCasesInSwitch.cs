﻿using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSonarQubeAnalyzer.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AtLeastThreeCasesInSwitch : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S1301";
        internal const string Description = "\"switch\" statements should have at least 3 \"case\" clauses";
        internal const string MessageFormat = "Replace this \"switch\" statement with \"if\" statements to increase readability.";
        internal const string Category = "SonarQube";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                c =>
                {
                    var switchNode = (SwitchStatementSyntax)c.Node;
                    if (!HasAtLeastThreeLabels(switchNode))
                    {
                        c.ReportDiagnostic(Diagnostic.Create(Rule, c.Node.GetLocation()));
                    }
                },
                SyntaxKind.SwitchStatement);
        }

        private static bool HasAtLeastThreeLabels(SwitchStatementSyntax node)
        {
            return node.Sections.Sum(section => section.Labels.Count) >= 3;
        }
    }
}
