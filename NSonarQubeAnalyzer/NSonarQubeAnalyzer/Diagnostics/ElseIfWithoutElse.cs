﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSonarQubeAnalyzer.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ElseIfWithoutElse : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S126";
        internal const string Description = "\"if ... else if\" constructs shall be terminated with an \"else\" clause";
        internal const string MessageFormat = "Add the missing \"else\" clause.";
        internal const string Category = "SonarQube";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public ImmutableArray<SyntaxKind> SyntaxKindsOfInterest { get { return ImmutableArray.Create(SyntaxKind.IfStatement); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(
                c =>
                {
                    var ifNode = (IfStatementSyntax)c.Node;
                    if (IsElseIfWithoutElse(ifNode))
                    {
                        c.ReportDiagnostic(Diagnostic.Create(Rule, ifNode.GetLocation()));
                    }
                },
                SyntaxKind.IfStatement);
        }

        private static bool IsElseIfWithoutElse(IfStatementSyntax node)
        {
            return node.Parent.IsKind(SyntaxKind.ElseClause) &&
                node.Else == null;
        }
    }
}
