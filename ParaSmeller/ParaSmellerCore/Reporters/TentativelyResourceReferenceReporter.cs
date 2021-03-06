﻿using System.Linq;
using Microsoft.CodeAnalysis;
using ParaSmellerCore.Diagnostics;
using ParaSmellerCore.Representation;
using ParaSmellerCore.SemanticAnalysis;
using Diagnostic = ParaSmellerCore.Diagnostics.Diagnostic;

namespace ParaSmellerCore.Reporters
{
    public class TentativelyResourceReferenceReporter: BaseReporter
    {
        public const string DiagnosticId = "TRR001";

        private const string MonitorWait = "System.Threading.Monitor.Wait";
        private const string MonitorTryEnter = "System.Threading.Monitor.TryEnter";
        private const string WaitHandleWaitOne = "System.Threading.WaitHandle.WaitOne";
        private const string WaitHandleWaitAll = "System.Threading.WaitHandle.WaitAll";
        private const string WaitHandleWaitAny = "System.Threading.WaitHandle.WaitAny";
        private const string SpinLockTryEnter = "System.Threading.SpinLock.TryEnter";
        private const string BarrierSignalAndWait = "System.Threading.Barrier.SignalAndWait";

        private static readonly string[] TimeoutTypes = { "TimeSpan", "Int32" };
        private static readonly string[] NotAllowedApis = { MonitorWait, WaitHandleWaitOne, WaitHandleWaitAll, WaitHandleWaitAny, MonitorTryEnter, SpinLockTryEnter, BarrierSignalAndWait };

        public static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.TRRAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString MessageFormatTentativelyResourceReference = new LocalizableResourceString(nameof(Resources.TRRAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        public static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.TRRAnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private void CheckForNotAllowedApiUsages(Member member)
        {
            var invocationsToReport = member.GetAllInvocations().Where(e => NotAllowedApis.Contains(e.MethodDefinitionWithoutParameters));
            foreach (var invocationToReport in invocationsToReport)
            {
                var symbol = SymbolInspector.GetSpecializedSymbol<IMethodSymbol>(invocationToReport.Implementation, member.ContainingClass.SemanticModel);

                if (ContainsTimeout(symbol))
                {
                    Reports.Add(ReportTimeoutUsage(invocationToReport.Implementation));
                }
            }
        }

        private static bool ContainsTimeout(IMethodSymbol methodSymbol)
        {
            var parameters = methodSymbol.Parameters;
            foreach (var parameter in parameters)
            {
                if (TimeoutTypes.Contains(parameter.Type.Name))
                {
                    return true;
                }
            }
            return false;
        }

        private static Diagnostic ReportTimeoutUsage(SyntaxNode syntaxnode)
        {
            return new Diagnostic(DiagnosticId, Title, MessageFormatTentativelyResourceReference, Description, DiagnosticCategory.Synchronization, syntaxnode.GetLocation());
        }

        protected override void Register()
        {
            RegisterMethodReport(CheckForNotAllowedApiUsages);
        }
    }
}
