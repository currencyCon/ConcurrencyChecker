﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConcurrencyAnalyzer.Representation
{
    public abstract class Body
    {
        public readonly SyntaxNode Implementation;
        public readonly Member ContainingMember;
        public readonly ICollection<InvocationExpressionRepresentation> InvocationExpressions = new List<InvocationExpressionRepresentation>();
        public readonly ICollection<Body> Blocks = new List<Body>();
        public abstract bool IsSynchronized { get; }

        protected Body(Member member, SyntaxNode implementation)
        {
            Implementation = implementation;
            ContainingMember = member;
        }

        public ICollection<InvocationExpressionRepresentation> GetAllInvocations()
        {
            var invocations = InvocationExpressions.ToList();
            foreach (var block in Blocks)
            {
                invocations.AddRange(block.GetAllInvocations());
            }
            return invocations;
        }

        public void AppendLockArguments(ICollection<string> lockObjects)
        {
            if (IsSynchronized)
            {
                lockObjects.Add(((LockStatementSyntax) Implementation).Expression.ToString());
            }
            foreach (var subLockBlock in Blocks)
            {
                subLockBlock.AppendLockArguments(lockObjects);
            }
        }

        public void AddInvokedMembersWithLock(ICollection<Member> members, Member member)
        {
            if (!members.Contains(member) && IsSynchronized)
            {
                members.Add(member);
            }

            foreach (var subBlock in Blocks)
            {
                subBlock.AddInvokedMembersWithLock(members, member);
            }
        }
    }
}
