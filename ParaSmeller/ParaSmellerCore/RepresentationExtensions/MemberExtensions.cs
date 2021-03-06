﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParaSmellerCore.Representation;

namespace ParaSmellerCore.RepresentationExtensions
{
    public static class MemberExtensions
    {
        public static IEnumerable<LockStatementSyntax> GetLockStatements(this Member member)
        {
            return GetLockStatements(member.Blocks);
        }

        private static List<LockStatementSyntax> GetLockStatements(IEnumerable<Body> bodies)
        {
            var lockStatementSyntaxs = new List<LockStatementSyntax>();
            foreach (var body in bodies)
            {
                if (body.IsSynchronized)
                {
                    lockStatementSyntaxs.Add(body.Implementation as LockStatementSyntax);
                }
                lockStatementSyntaxs.AddRange(GetLockStatements(body.Blocks));
            }
            return lockStatementSyntaxs;
        }
    }
}