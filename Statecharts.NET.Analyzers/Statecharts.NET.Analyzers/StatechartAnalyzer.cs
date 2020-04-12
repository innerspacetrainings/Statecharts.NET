using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using static Statecharts.NET.Analyzers.Helpers;

namespace Statecharts.NET.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StatechartAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Statechart";

        private static readonly DiagnosticDescriptor NoChildTransitionOnAtomicStatenode =
            CreateRule(AnalyzerIds.StatechartDefinitionParser, Category, DiagnosticSeverity.Error, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(NoChildTransitionOnAtomicStatenode);

        public override void Initialize(AnalysisContext context)
        {
            //context.EnableConcurrentExecution();
            //context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationStartAction(AnalyzeCompilation);
        }

        private void AnalyzeCompilation(CompilationStartAnalysisContext compilationContext)
        {
            var statechartDefinitionType = compilationContext.Compilation.GetTypeByMetadataName("Statecharts.NET.Model.StatechartDefinition`1");
            if (statechartDefinitionType == null) return;

            compilationContext.RegisterOperationAction(context =>
            {
                var invocationOperation = (IInvocationOperation)context.Operation;
                var isStatechartDefinition = ((INamedTypeSymbol)invocationOperation.Type).ConstructedFrom.Equals(statechartDefinitionType);
                if (!isStatechartDefinition) return;

                var rootNodeSyntax = (ArgumentSyntax)invocationOperation.Arguments.FirstOrDefault().Syntax;
                var isRootNodeInvocationExpression = rootNodeSyntax.Expression.IsKind(SyntaxKind.InvocationExpression);

                var rootExpression = ((InvocationExpressionSyntax) rootNodeSyntax.Expression);

                //var test = context.Compilation.GetSemanticModel(rootExpression.SyntaxTree).GetSymbolInfo(rootExpression);
                //var test2 = context.Compilation.GetSemanticModel(rootExpression.SyntaxTree).GetTypeInfo(rootExpression);
                //var test3 = context.Compilation.GetSemanticModel(rootExpression.SyntaxTree).GetOperation(rootExpression);

                async System.Threading.Tasks.Task<object> Test()
                {
                    const string noContext = @"
internal class NoContext : Statecharts.NET.Interfaces.IContext<NoContext>
{
    public bool Equals(NoContext other) => true;
    public NoContext CopyDeep() => new NoContext();
}";

                    var options = ScriptOptions.Default
                        .WithReferences(Path.Combine(Environment.CurrentDirectory, "Statecharts.NET.Language.dll"))
                        .WithImports(
                            "Statecharts.NET.Model",
                            "Statecharts.NET.Language",
                            "Statecharts.NET.Language.Keywords");

                    return await CSharpScript.EvaluateAsync($"{noContext}{Environment.NewLine}{invocationOperation.Syntax}", options);
                }

                var a = Test().Result;
                ;
            }, OperationKind.Invocation);
        }
    }
}
