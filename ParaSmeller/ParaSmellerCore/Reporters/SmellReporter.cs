﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ParaSmellerCore.Diagnostics;
using ParaSmellerCore.RepresentationFactories;
using Diagnostic = ParaSmellerCore.Diagnostics.Diagnostic;

namespace ParaSmellerCore.Reporters
{
    public class SmellReporter
    {
        public static readonly ICollection<Smell> DefaultSmellCollection = new List<Smell>
        {
            Smell.PrimitiveSynchronization,
            Smell.FireAndForget,
            Smell.Finalizer,
            Smell.HalfSynchronized,
            Smell.MonitorWaitOrSignal,
            Smell.ExplicitThreads,
            Smell.NestedSynchronization,
            Smell.OverAsynchrony,
            Smell.TenativelyRessource,
            Smell.WaitingConditionsTasks
        };

        private readonly Dictionary<Smell, BaseReporter> _reporters = new Dictionary<Smell, BaseReporter>
        {
            {Smell.PrimitiveSynchronization, new PrimitiveSynchronizationReporter()},
            {Smell.FireAndForget, new FireAndForgetReporter() },
            {Smell.Finalizer, new FinalizerReporter() },
            {Smell.HalfSynchronized, new HalfSynchronizedReporter() },
            {Smell.MonitorWaitOrSignal, new MonitorOrWaitSignalReporter() },
            {Smell.ExplicitThreads, new ExplicitThreadsReporter()},
            {Smell.NestedSynchronization, new NestedSynchronizedMethodClassReporter() },
            {Smell.OverAsynchrony, new OverAsynchronyReporter() },
            {Smell.TenativelyRessource, new TentativelyResourceReferenceReporter() },
            {Smell.WaitingConditionsTasks, new WaitingConditionsTasksReporter() }
        };

        public List<Diagnostic> Report(Compilation compilation, ICollection<Smell> smells)
        {
            var solutionModel = SolutionRepresentationFactory.Create(compilation);
            var diagnostics = new List<Diagnostic>();
            foreach (var smell in smells)
            {
                Logger.Debug("Executing Reporter:" + _reporters[smell].GetType().Name);
                diagnostics.AddRange(_reporters[smell].Report(solutionModel));
            }
            ReportAnalysisEnd();
            return diagnostics;
        }

        private static void ReportAnalysisEnd()
        {
            Logger.Debug("=================");
            Logger.Debug("Analysis finished");
            Logger.Debug("=================");
        }

    }
}
