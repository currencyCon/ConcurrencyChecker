﻿using System.Collections.Generic;
using System.Linq;
using ConcurrencyAnalyzer.Representation;
using Microsoft.CodeAnalysis;
using Diagnostic = ConcurrencyAnalyzer.Diagnostics.Diagnostic;

namespace ConcurrencyAnalyzer.Reporters
{
    public class OverAsynchronyReporter: BaseReporter
    {
        public const string Category = "Synchronization";
        public const string DiagnosticId = "OA001";
        public const string DiagnosticIdNestedAsync = "OA002";
        private const int DepthAsyncTillWarning = 2;

        public static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.OAAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.OAAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString MessageFormatNestedAsync = new LocalizableResourceString(nameof(Resources.OAAnalyzerMessageFormatNestedAsync), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.OAAnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private static bool CheckForNestedAsync(MethodRepresentation method, int counter)
        {
            var symbol = method.ContainingClass.SemanticModel.GetDeclaredSymbol(method.Implementation) as IMethodSymbol;
            
            if (symbol != null && symbol.IsAsync)
            {
                if (counter >= DepthAsyncTillWarning)
                {
                    return true;
                }
                counter++;
                var invocations = method.GetAllInvocations();
                foreach (var invocation in invocations.Where(i => i.InvokedImplementation is MethodRepresentation).Select(i => i.InvokedImplementation as MethodRepresentation))
                {
                    if (CheckForNestedAsync(invocation, counter))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CheckForPrivateAsync(MethodRepresentation method)
        {
            var symbol = method.ContainingClass.SemanticModel.GetDeclaredSymbol(method.Implementation) as IMethodSymbol;

            if (symbol != null && symbol.IsAsync && symbol.DeclaredAccessibility != Accessibility.Public)
            {
                Reports.Add(new Diagnostic(DiagnosticId, Title, MessageFormat, Description, Category, method.Implementation.GetLocation()));
            }
        }

        protected override void Register()
        {
            RegisterMethodReport(CheckMethodForAsynchronicity);
        }

        private void CheckMethodForAsynchronicity(MethodRepresentation method)
        {
            CheckForPrivateAsync(method);
            if (CheckForNestedAsync(method, 0))
            {
                Reports.Add(new Diagnostic(DiagnosticIdNestedAsync, Title, MessageFormatNestedAsync, Description, Category,
                    method.Implementation.GetLocation(), new object[] {DepthAsyncTillWarning + 1}));
            }
        }
    }
}