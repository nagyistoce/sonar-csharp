﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSonarQubeAnalyzer.Diagnostics;

namespace Tests.Diagnostics
{
    [TestClass]
    public class SwitchWithoutDefaultTest
    {
        [TestMethod]
        public void SwitchWithoutDefault()
        {
            Verifier.Verify(@"TestCases\SwitchWithoutDefault.cs", new SwitchWithoutDefault());
        }
    }
}
