﻿using System.Collections.Generic;
using System.Linq;
using ConcurrencyAnalyzer.Builders;
using ConcurrencyAnalyzer.SyntaxNodeUtils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConcurrencyAnalyzer.Representation
{
    public class InterfaceRepresentation
    {
        public readonly InterfaceDeclarationSyntax Implementation;
        public readonly SyntaxToken Name;
        public readonly ICollection<Member> Members;
		public readonly SemanticModel SemanticModel;
        
        public ICollection<MethodRepresentation> Methods => Members.OfType<MethodRepresentation>().ToList();
        public ICollection<PropertyRepresentation> Properties => Members.OfType<PropertyRepresentation>().ToList();
        public INamedTypeSymbol NamedTypeSymbol { get; set; }

        public InterfaceRepresentation(InterfaceDeclarationSyntax interfaceDeclarationSyntax, SemanticModel semanticModel)
        {
            Name = interfaceDeclarationSyntax.Identifier;
            SemanticModel = semanticModel;
            Members = new List<Member>();
            Implementation = interfaceDeclarationSyntax;
            NamedTypeSymbol = semanticModel.GetDeclaredSymbol(Implementation);
        }
        
    }
}
