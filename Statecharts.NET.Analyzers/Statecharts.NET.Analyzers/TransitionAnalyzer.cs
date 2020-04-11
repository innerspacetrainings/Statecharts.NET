using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;
using static Statecharts.NET.Analyzers.Helpers;

namespace Statecharts.NET.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TransitionAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Transition";

        private static readonly DiagnosticDescriptor NoChildTransitionOnAtomicStatenode =
            CreateRule(AnalyzerIds.NoChildTransitionOnAtomicStatenode, Category, DiagnosticSeverity.Error, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(NoChildTransitionOnAtomicStatenode);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterOperationAction(AnalyzeChildTransition, OperationKind.Invocation);
        }

        private static void AnalyzeChildTransition(OperationAnalysisContext context)
        {
            var invocationOperation = (IInvocationOperation)context.Operation;
            if (!invocationOperation.Syntax.Parent.IsKind(SyntaxKind.Argument)) return;

            var atomicStateNodeDefinitionType = context.Compilation.GetTypeByMetadataName("Statecharts.NET.Model.IAtomicStatenodeDefinition");
            if (atomicStateNodeDefinitionType == null) return;

            var isAtomicStatenodeDefinition = invocationOperation.Type.AllInterfaces.Any(i => i.Equals(atomicStateNodeDefinitionType));
            if (!isAtomicStatenodeDefinition) return;

            var statenodeName = ((LiteralExpressionSyntax)invocationOperation.Syntax.DescendantNodes()
                .FirstOrDefault(node => node.IsKind(SyntaxKind.StringLiteralExpression)))?.Token.ValueText;

            var childTransitionExpressions = invocationOperation
                .Descendants()
                .OfType<IInvocationOperation>()
                .Where(invocation => invocation.TargetMethod.Name == "Child")
                .Select(operation => (InvocationExpressionSyntax)operation.Syntax);

            foreach (var expression in childTransitionExpressions)
            {
                var location = Location.Create(
                    expression.SyntaxTree,
                    TextSpan.FromBounds(
                        expression.ArgumentList.SpanStart - 5, // 5: "Child"
                        expression.ArgumentList.CloseParenToken.Span.End));

                context.ReportDiagnostic(Diagnostic.Create(NoChildTransitionOnAtomicStatenode, location, statenodeName));
            }
        }
    }
}
