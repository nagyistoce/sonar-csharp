﻿using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSonarQubeAnalyzer.Diagnostics
{
    public class CommentRegularExpressionRule
    {
        public DiagnosticDescriptor Descriptor;
        public string RegularExpression;
    }

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommentRegularExpression : DiagnosticAnalyzer
    {
        public ImmutableArray<CommentRegularExpressionRule> Rules;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return Rules.Select(r => r.Descriptor).ToImmutableArray(); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(
                c =>
                {
                    var comments = from t in c.Tree.GetCompilationUnitRoot().DescendantTrivia()
                                     where IsComment(t)
                                     select t;

                    foreach (var comment in comments)
                    {
                        var text = comment.ToString();
                        foreach (var rule in Rules.Where(rule => Regex.IsMatch(text, rule.RegularExpression)))
                        {
                            c.ReportDiagnostic(Diagnostic.Create(rule.Descriptor, comment.GetLocation()));
                        }
                    }
                });
        }

        private static bool IsComment(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                trivia.IsKind(SyntaxKind.MultiLineCommentTrivia) ||
                trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
                trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia);
        }
    }
}
