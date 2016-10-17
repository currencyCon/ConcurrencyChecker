﻿using ConcurrencyAnalyzer.Representation;
using ConcurrencyAnalyzer.SyntaxFilters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConcurrencyAnalyzer.RepresentationFactories
{
    public class InvocationExpressionRepresentationFactory
    {

        public static InvocationExpressionRepresentation Create(InvocationExpressionSyntax invocationExpressionSyntax, SemanticModel semanticModel, IBody containingBody = null)
        {
            var invocationExpression = (MemberAccessExpressionSyntax) invocationExpressionSyntax.Expression;
            var className = (IdentifierNameSyntax) invocationExpression.Expression;
            var invocationTarget = invocationExpression.Name;
            var methodInfo = semanticModel.GetSymbolInfo(invocationTarget);
            var type = methodInfo.Symbol.Kind;
            
            var invocation = new InvocationExpressionRepresentation
            {
                Type = type,
                Implementation = invocationExpressionSyntax,
                ContainingBody = containingBody,
                Synchronized = containingBody?.Implementation.IsSynchronized() ?? false,
                InvocationTargetName = invocationTarget,
                CalledClass = className
            };

            return invocation;
        }
    }
}
