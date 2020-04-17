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
using Statecharts.NET.Interfaces;
using Statecharts.NET.Language;
using Statecharts.NET.Language.Builders.StateNode;
using Statecharts.NET.Model;
using Statecharts.NET.XState;
using static Statecharts.NET.Analyzers.Helpers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Statecharts.NET.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StatechartAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Statechart";

        private static readonly DiagnosticDescriptor StatechartDefinition =
            CreateRule(AnalyzerIds.StatechartDefinitionParser, Category, DiagnosticSeverity.Info, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(StatechartDefinition);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationStartAction(AnalyzeCompilation);
        }

        class __StatechartAnalyzer_DummyContext : IContext<__StatechartAnalyzer_DummyContext>, IXStateSerializable
        {
            public bool Equals(__StatechartAnalyzer_DummyContext other) => true;
            public __StatechartAnalyzer_DummyContext CopyDeep() => new __StatechartAnalyzer_DummyContext();
            public ObjectValue AsJSObject() => new ObjectValue(Enumerable.Empty<JSProperty>());
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

                var rootExpression = (InvocationExpressionSyntax) rootNodeSyntax.Expression;

                var initialContextSyntaxNode = ((ObjectCreationExpressionSyntax)((ArgumentSyntax)((IInvocationOperation)invocationOperation.Children.FirstOrDefault()).Arguments.FirstOrDefault().Syntax).Expression);

                var test = (TypeSyntax)IdentifierName("__StatechartAnalayzer_DummyContext");
                var test2 = IdentifierName(Identifier("__StatechartAnalayzer_DummyContext"));

                var res = initialContextSyntaxNode.WithType(test);
                ;

                //var test = context.Compilation.GetSemanticModel(rootExpression.SyntaxTree).GetSymbolInfo(rootExpression);
                //var test2 = context.Compilation.GetSemanticModel(rootExpression.SyntaxTree).GetTypeInfo(rootExpression);
                //var test3 = context.Compilation.GetSemanticModel(rootExpression.SyntaxTree).GetOperation(rootExpression);

                async System.Threading.Tasks.Task<object> Test()
                {
                    var options = ScriptOptions.Default
                        .WithReferences(typeof(Statechart).Assembly.Location)
                        .WithImports(
                            "Statecharts.NET.Model",
                            "Statecharts.NET.Language",
                            "Statecharts.NET.Language.Keywords");

                    return await CSharpScript.EvaluateAsync(rootExpression.ToString(), options);
                }

                var rootNode = Test().Result as CompoundStatenodeDefinition;
                var statechart = Statechart
                    .WithInitialContext(new __StatechartAnalyzer_DummyContext())
                    .WithRootState(rootNode);
                context.ReportDiagnostic(Diagnostic.Create(StatechartDefinition, invocationOperation.Syntax.GetLocation(), statechart.AsXStateVisualizerV4Definition()));
            }, OperationKind.Invocation);
        }
    }
}
