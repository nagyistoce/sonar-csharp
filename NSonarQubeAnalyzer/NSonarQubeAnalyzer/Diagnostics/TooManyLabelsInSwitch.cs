﻿using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSonarQubeAnalyzer.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TooManyLabelsInSwitch : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S1479";
        internal const string Description = "\"switch\" statements should not have too many \"case\" clauses";
        internal const string MessageFormat = "Reduce the number of switch cases from {1} to at most {0}.";
        internal const string Category = "SonarQube";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public int Maximum;

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                c =>
                {
                    var switchNode = (SwitchStatementSyntax)c.Node;
                    var labels = NumberOfLabels(switchNode);

                    if (labels > Maximum)
                    {
                        c.ReportDiagnostic(Diagnostic.Create(Rule, switchNode.GetLocation(), Maximum, labels));
                    }
                },
                SyntaxKind.SwitchStatement);
        }

        private static int NumberOfLabels(SwitchStatementSyntax node)
        {
            return node.Sections.Sum(e => e.Labels.Count);
        }
    }
}
