﻿using System.Collections.Generic;
using ConcurrencyAnalyzer.SymbolExtensions;
using Microsoft.CodeAnalysis;

namespace ConcurrencyAnalyzer.Hierarchy
{
    public class Hierarchy
    {
        readonly List<string> _inheritanceClasses = new List<string>();
        public List<INamedTypeSymbol> InheritanceFromInterfaces { get; }
        public List<ITypeSymbol> InheritanceFromClass { get; }

        public Hierarchy(ITypeSymbol type)
        {
            var baseTypes = type.GetBaseTypesAndThis();
            var interfaces = type.AllInterfaces;

            InheritanceFromInterfaces = new List<INamedTypeSymbol>();
            InheritanceFromClass = new List<ITypeSymbol>();
            foreach (var interfacee in interfaces)
            {
                _inheritanceClasses.Add(interfacee.Name);
                InheritanceFromInterfaces.Add(interfacee);
            }
            foreach (var baseType in baseTypes)
            {
                _inheritanceClasses.Add(baseType.Name);
                InheritanceFromClass.Add(baseType);
            }

            InheritanceFromClass.RemoveAt(0);
        }

        public bool IsSubClass(ITypeSymbol baseType)
        {
            return _inheritanceClasses.Contains(baseType.Name);
        }
    }
}