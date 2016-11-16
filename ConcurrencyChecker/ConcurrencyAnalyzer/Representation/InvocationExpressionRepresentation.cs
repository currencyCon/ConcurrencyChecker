﻿using System.Collections.Generic;
using ConcurrencyAnalyzer.SemanticAnalysis;
using ConcurrencyAnalyzer.SyntaxNodeUtils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConcurrencyAnalyzer.Representation
{
    public class InvocationExpressionRepresentation
    {
        public readonly string CalledClass;
        public readonly SimpleNameSyntax InvocationTargetName;
        public readonly bool IsSynchronized;
        public readonly InvocationExpressionSyntax Implementation;
        public readonly Body ContainingBody;
        public readonly SymbolKind Type;
        public readonly List<IdentifierNameSyntax> Arguments;
        public readonly string OriginalDefinition;
        public readonly string Defintion;
        public readonly bool IsInvokedInTask;
        public readonly ICollection<Member> InvokedImplementations;
        
        public InvocationExpressionRepresentation(bool isSynchronized, SymbolInformation symbolInfo, InvocationExpressionSyntax implementation, Body containingBody, SimpleNameSyntax invocationTarget, bool isInvokedInTask)
        {
            IsSynchronized = isSynchronized;
            Arguments = new List<IdentifierNameSyntax>();
            CalledClass = symbolInfo.ClassName;
            OriginalDefinition = symbolInfo.OriginalDefinition;
            Type = symbolInfo.Type;
            Implementation = implementation;
            ContainingBody = containingBody;
            InvocationTargetName = invocationTarget;
            IsInvokedInTask = isInvokedInTask;
            InvokedImplementations = new List<Member>();
            Defintion = symbolInfo.Definition;
        }

        public TParent GetFirstParent<TParent>()
        {
            return Implementation.GetFirstParent<TParent>();
        }
    }
}
