﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSonarQubeAnalyzer.Diagnostics
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommentedOutCode : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "CommentedCode";
        internal const string Description = "Comment should not include code";
        internal const string MessageFormat = "Remove this commented out code or move it into XML documentation.";
        internal const string Category = "SonarQube";
        internal const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Description, MessageFormat, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(
                c =>
                {
                    foreach (var token in c.Tree.GetRoot().DescendantTokens())
                    {
                        Action<IEnumerable<SyntaxTrivia>> check =
                            trivias =>
                            {
                                var lastCommentedCodeLine = int.MinValue;

                                foreach (var trivia in trivias)
                                {
                                    if (!trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) &&
                                        !trivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
                                    {
                                        continue;
                                    }

                                    var contents = trivia.ToString().Substring(2);
                                    if (trivia.IsKind(SyntaxKind.MultiLineCommentTrivia))
                                    {
                                        contents = contents.Substring(0, contents.Length - 2);
                                    }

                                    var baseLineNumber = trivia.GetLocation().GetLineSpan().StartLinePosition.Line;
                                    // TODO Do not duplicate line terminators here
                                    var lines = contents.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                                    for (var offset = 0; offset < lines.Length; offset++)
                                    {
                                        if (!IsCode(lines[offset]))
                                        {
                                            continue;
                                        }

                                        var lineNumber = baseLineNumber + offset;
                                        var oldLastCommentedCodeLine = lastCommentedCodeLine;
                                        lastCommentedCodeLine = lineNumber;

                                        if (lineNumber == oldLastCommentedCodeLine + 1)
                                        {
                                            continue;
                                        }

                                        var location = Location.Create(c.Tree, c.Tree.GetText().Lines[lineNumber].Span);
                                        c.ReportDiagnostic(Diagnostic.Create(Rule, location));
                                        break;
                                    }
                                }
                            };

                        check(token.LeadingTrivia);
                        check(token.TrailingTrivia);
                    }
                });
        }

        private static bool IsCode(string line)
        {
            line = line.Replace(" ", "").Replace("\t", "");

            return line.EndsWith(";") ||
                line.EndsWith("{") ||
                line.EndsWith("}") ||
                line.Contains("++") ||
                line.Contains("for(") ||
                line.Contains("if(") ||
                line.Contains("while(") ||
                line.Contains("catch(") ||
                line.Contains("switch(") ||
                line.Contains("try{") ||
                line.Contains("else{") ||
                (line.Length - line.Replace("&&", "").Replace("||", "").Length) / 2 >= 3;
        }
    }
}
