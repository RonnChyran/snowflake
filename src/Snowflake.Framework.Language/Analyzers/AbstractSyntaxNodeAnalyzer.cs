﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Linq;
using System.Threading;

namespace Snowflake.Language.Analyzers
{
    public abstract class AbstractSyntaxNodeAnalyzer
        : DiagnosticAnalyzer
    {
        protected abstract IEnumerable<SyntaxKind> Kinds { get; }
        public abstract IEnumerable<Diagnostic> Analyze(Compilation compilation, SemanticModel semanticModel, SyntaxNode node, CancellationToken cancel = default);
        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(this.Analyze, this.Kinds.ToImmutableArray());
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            // Sometimes we want to ignore analyzers for tests. This is usually not the case.
            // A third-party consumer would have to try real hard to trigger this behaviour..
            if (context.Compilation.AssemblyName == "Snowflake.Framework.Tests" 
                && context.Compilation.GetTypeByMetadataName("Snowflake.Tests.SnowflakeInternalUnsafeIgnoreAnalyzerForTestsAttribute") is INamedTypeSymbol specialTest)
            {
                if (context.SemanticModel.GetSymbolInfo(context.Node).Symbol?.GetAttributes()
                    .Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, specialTest)) == true)
                    return;
            }

            foreach (var diagnostic in Analyze(context.Compilation, context.SemanticModel, context.Node, context.CancellationToken))
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
