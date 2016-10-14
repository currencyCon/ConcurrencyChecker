﻿using System.Collections.Generic;
using System.Linq;
using ConcurrencyAnalyzer.SyntaxFilters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConcurrencyAnalyzer.Representation
{
    public class ClassRepresentation
    {
        public ClassDeclarationSyntax ClassDeclarationSyntax;
        public string FullyQualifiedDomainName;
        public SyntaxToken Name { get; set; }
        private ICollection<IMethodRepresentation> _synchronizedMethods;

        public ICollection<IMethodRepresentation> SynchronizedMethods
        {
            get
            {
                return _synchronizedMethods ??
                       (_synchronizedMethods = Members.Where(e => e is MethodRepresentation && e.IsFullySynchronized())
                           .Select(e => e as IMethodRepresentation).ToList());
            }
        }

        private ICollection<IMethodRepresentation> _unSynchronizedMethods;

        public ICollection<IMethodRepresentation> UnSynchronizedMethods
        {
            get
            {
                return _unSynchronizedMethods ??
                       (_unSynchronizedMethods = Members.Where(e => e is MethodRepresentation && !e.IsFullySynchronized())
                           .Select(e => e as IMethodRepresentation).ToList());
            }
        }




        private ICollection<IPropertyRepresentation> _synchronizedProperties;

        public ICollection<IPropertyRepresentation> SynchronizedProperties
        {
            get
            {
                return _synchronizedProperties ??
                       (_synchronizedProperties = Members.Where(e => e is PropertyRepresentation && e.IsFullySynchronized())
                           .Select(e => e as IPropertyRepresentation).ToList());
            }
        }

        private ICollection<IPropertyRepresentation> _unSynchronizedProperties;

        public ICollection<IPropertyRepresentation> UnSynchronizedProperties
        {
            get
            {
                return _unSynchronizedProperties ??
                       (_unSynchronizedProperties = Members.Where(e => e is PropertyRepresentation && !e.IsFullySynchronized())
                           .Select(e => e as IPropertyRepresentation).ToList());
            }
        }


        public ICollection<IMethodRepresentation> Methods { get; set; }
        public ICollection<IPropertyRepresentation> Properties { get; set; }
        public ICollection<IMemberWithBody> Members { get; set; }
        public ClassRepresentation(ClassDeclarationSyntax classDeclarationSyntax)
        {
            Name = classDeclarationSyntax.Identifier;
            Methods = new List<IMethodRepresentation>();
            Properties = new List<IPropertyRepresentation>();
            Members = new List<IMemberWithBody>();
            ClassDeclarationSyntax = classDeclarationSyntax;
            FullyQualifiedDomainName = classDeclarationSyntax.Identifier.ToFullString();
        }

        public IEnumerable<InvocationExpressionRepresentation> GetInvocationsInLocks()
        {
            IEnumerable<InvocationExpressionRepresentation> invocations = new List<InvocationExpressionRepresentation>();

            foreach (var memberWithBody in Members)
            {
                invocations = invocations.Concat(SyntaxNodeFilter.GetInvocationsInLocks(memberWithBody.Blocks));
            }
            return invocations;
        }

        public IEnumerable<IdentifierNameSyntax> GetIdentifiersInLocks()
        {
            IEnumerable<IdentifierNameSyntax> identifiers = new List<IdentifierNameSyntax>();

            foreach (var memberWithBody in Members)
            {
                identifiers = identifiers.Concat(SyntaxNodeFilter.GetIdentifiersInLocks(memberWithBody.Blocks));
            }
            return identifiers;
        }
    }
}
