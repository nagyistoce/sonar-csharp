﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSonarQubeAnalyzer.Diagnostics;

namespace Tests.Diagnostics
{
    [TestClass]
    public class CommentRegularExpressionTest
    {
        [TestMethod]
        public void CommentRegularExpression()
        {
            var rules = ImmutableArray.Create(
                new CommentRegularExpressionRule
                {
                    Descriptor = new DiagnosticDescriptor("id1", "", "", "SonarQube", DiagnosticSeverity.Warning, true),
                    RegularExpression = "(?i)TODO"
                });

            var diagnostic = new CommentRegularExpression {Rules = rules};
            Verifier.Verify(@"TestCases\CommentRegularExpression.cs", diagnostic);
        }
    }
}
