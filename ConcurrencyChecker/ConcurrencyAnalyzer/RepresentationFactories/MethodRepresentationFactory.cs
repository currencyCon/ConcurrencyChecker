﻿using System.Collections.Generic;
using System.Linq;
using ConcurrencyAnalyzer.Representation;
using ConcurrencyAnalyzer.SyntaxFilters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConcurrencyAnalyzer.RepresentationFactories
{
    public static class MethodRepresentationFactory
    {

        public static MethodRepresentation Create(MethodDeclarationSyntax methodDeclarationSyntax, ClassRepresentation classRepresentation, SemanticModel semanticModel)
        {
            var methodRepresentation = new MethodRepresentation(methodDeclarationSyntax, classRepresentation);
            AddBaseBody(methodRepresentation, semanticModel);
            AddDirectInvcocations(methodRepresentation, semanticModel);
            return methodRepresentation;
        }

        private static void AddBaseBody(MethodRepresentation methodRepresentation, SemanticModel semanticModel)
        {
            var baseBody = BlockRepresentationFactory.Create(methodRepresentation.Implementation.Body,
                methodRepresentation, semanticModel);
            methodRepresentation.Blocks.Add(baseBody);
        }

        private static void AddDirectInvcocations(MethodRepresentation methodRepresentation, SemanticModel semanticModel)
        {
            foreach (var invocationExpressionSyntax in methodRepresentation.Implementation.Body.Statements.Where(IsTopLevelStatement).SelectMany(e => e.GetChildren<InvocationExpressionSyntax>()))
            {
                if (!(invocationExpressionSyntax.Parent is ParenthesizedLambdaExpressionSyntax))
                {
                    methodRepresentation.InvocationExpressions.Add(InvocationExpressionRepresentationFactory.Create(invocationExpressionSyntax, semanticModel, methodRepresentation.Blocks.First()));
                }
            }
        }

        private static bool IsTopLevelStatement(StatementSyntax e)
        {
            return !(e is LockStatementSyntax) && ! (e is BlockSyntax);
        }
    }
}
