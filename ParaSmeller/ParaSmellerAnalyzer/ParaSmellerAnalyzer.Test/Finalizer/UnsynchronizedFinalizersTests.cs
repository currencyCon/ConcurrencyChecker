﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParaSmeller.Test.Verifiers;
using ParaSmellerCore.Reporters;
using CodeFixVerifier = ParaSmeller.Test.Verifiers.CodeFixVerifier;

namespace ParaSmeller.Test.Finalizer
{
    [TestClass]
    public class UnsynchronizedFinalizersTests: CodeFixVerifier
    {
        [TestMethod]
        public void TestDoesNotReportFalsePositivesOnCorrectLock()
        {
            const string test = @"
namespace ParaSmeller.Test.TestCodeTester
{
    public class SynchronizedFinalizer
    {
        public static int Counter;
        private static readonly object LockObject = new object();
        public SynchronizedFinalizer()
        {
            lock (LockObject)
            {
                Counter++;
            }
        }

        ~SynchronizedFinalizer()
        {
            lock (LockObject)
            {
                Counter--;
            }
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }
        [TestMethod]
        public void TestDoesNotReportOnReadOnlyFields()
        {
            const string test = @"
using ConcurrencyAnalyzer;

namespace ParaSmeller.Test.TestCodeTester
{
    public class SynchronizedFinalizer
    {
        private static readonly string LogMessage = ""Hello"";

        ~SynchronizedFinalizer()
        {
                Logger.DebugLog(LogMessage);
        }

    }
}";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestDoesNotReportOnReadOnlyFieldsComplexCase()
        {
            const string test = @"
using System;


namespace log4net.Appender
{

    public abstract class AppenderSkeleton 
    {
        protected AppenderSkeleton()
        {
        }

        ~AppenderSkeleton()
        {
            // An appender might be closed then garbage collected. 
            // There is no point in closing twice.
            if (!m_closed)
            {
                DoStuff(declaringType, ""Finalizing appender named["" +""blub"" + ""]."");
            }
        }


        public void DoStuff(Type type, string msg)
        {

        }

        private bool m_closed = false;

        public virtual bool Flush(int millisecondsTimeout)
        {
            return true;
        }

        private readonly static Type declaringType = typeof(AppenderSkeleton);
    }
}
";

            VerifyCSharpDiagnostic(test);
        }
        [TestMethod]
        public void TestReportsUnsynchronizedFieldAccess()
        {
            const string test = @"
namespace ParaSmeller.Test.TestCodeTester
{
    public class SynchronizedFinalizer
    {
        public static int Counter;
        public SynchronizedFinalizer()
        {
            Counter++;
        }

        ~SynchronizedFinalizer()
        {
            Counter--;
        }
    }
}";
            var expected = new [] {
                new DiagnosticResult{
                Id = FinalizerReporter.FinalizerSynchronizationDiagnosticId,
                Message = FinalizerReporter.MessageFormatFinalizerSynchronization.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 6, 9)
                        }
                },
                new DiagnosticResult{
                Id = FinalizerReporter.FinalizerSynchronizationDiagnosticId,
                Message = FinalizerReporter.MessageFormatFinalizerSynchronization.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 14, 13)
                        }
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void TestNoPositivesOnNonStaticFields()
        {
            const string test = @"
namespace ParaSmeller.Test.TestCodeTester
{
    public class SynchronizedFinalizer
    {
        public int Counter;
        public SynchronizedFinalizer()
        {
            Counter++;
        }

        ~SynchronizedFinalizer()
        {
            Counter--;
        }

    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestDoesNotReportFalsePositivesOnSynchronizedProperties()
        {
            const string test = @"
namespace ParaSmeller.Test.TestCodeTester
{
    public class SynchronizedFinalizer
    {
        private static int _counter;

        public static int Counter
        {
            get {
                lock (LockObject)
                {
                    return _counter;
                }
            } 
            set {
                lock (LockObject)
                {
                    _counter = value;
                }
            }
        }

        private static readonly object LockObject = new object();
        public SynchronizedFinalizer()
        {
            lock (LockObject)
            {
                Counter++;
            }
        }

        ~SynchronizedFinalizer()
        {
            lock (LockObject)
            {
                Counter--;
            }
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void TestReportsNotSynchronizedPropertyUsedInDestructor()
        {
            const string test = @"
namespace ParaSmeller.Test.TestCodeTester
{
    public class SynchronizedFinalizer
    {
        public static int Counter{get; set;}
        private static readonly object LockObject = new object();
        public SynchronizedFinalizer()
        {
            lock (LockObject)
            {
                Counter++;
            }
        }

        ~SynchronizedFinalizer()
        {
            lock (LockObject)
            {
                Counter--;
            }
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = FinalizerReporter.FinalizerSynchronizationDiagnosticId,
                Message = FinalizerReporter.MessageFormatFinalizerSynchronization.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 9)}
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void TestReportsUnsynchronizedPropertyAccess()
        {
            const string test = @"
namespace ParaSmeller.Test.TestCodeTester
{
    public class SynchronizedFinalizer
    {
        public static int Counter { get; set; }
        public SynchronizedFinalizer()
        {
            Counter++;
        }

        ~SynchronizedFinalizer()
        {
            Counter--;
        }
    }
}";
            var expected = new[] {
                new DiagnosticResult{
                Id = FinalizerReporter.FinalizerSynchronizationDiagnosticId,
                Message = FinalizerReporter.MessageFormatFinalizerSynchronization.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 6, 9)
                        }
                },
                new DiagnosticResult{
                Id = FinalizerReporter.FinalizerSynchronizationDiagnosticId,
                Message = FinalizerReporter.MessageFormatFinalizerSynchronization.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 14, 13)
                        }
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [TestMethod]
        public void TestReportsNonStaticLockObjects()
        {
            const string test = @"
namespace ParaSmeller.Test.TestCodeTester
{
    public class SynchronizedFinalizer
    {
        public static int Counter;
        public SynchronizedFinalizer()
        {
            lock (this)
            {
                Counter++;
            }
        }

        ~SynchronizedFinalizer()
        {
            lock (this)
            {
                Counter--;
            }
        }
    }
}";
            var expected = new[] {
                new DiagnosticResult{
                Id = FinalizerReporter.FinalizerSynchronizationDiagnosticId,
                Message = FinalizerReporter.MessageFormatFinalizerSynchronization.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 17)
                        }
                },
                new DiagnosticResult{
                Id = FinalizerReporter.FinalizerSynchronizationDiagnosticId,
                Message = FinalizerReporter.MessageFormatFinalizerSynchronization.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 19, 17)
                        }
                }
            };
            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void TestReportsDifferentLockObjects()
        {
            const string test = @"
namespace ParaSmeller.Test.TestCodeTester
{
    public class SynchronizedFinalizer
    {
        public static int Counter;
        private static readonly object LockObjectA = new object();
        private static readonly object LockObjectB = new object();

        public SynchronizedFinalizer()
        {
            lock (LockObjectA)
            {
                Counter++;
            }
        }

        ~SynchronizedFinalizer()
        {
            lock (LockObjectB)
            {
                Counter--;
            }
        }
    }
}";
            var expected = new[] {
                new DiagnosticResult{
                Id = FinalizerReporter.FinalizerSynchronizationDiagnosticId,
                Message = FinalizerReporter.MessageFormatFinalizerSynchronization.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 14, 17)
                        }
                },
                new DiagnosticResult{
                Id = FinalizerReporter.FinalizerSynchronizationDiagnosticId,
                Message = FinalizerReporter.MessageFormatFinalizerSynchronization.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 22, 17)
                        }
                }
            };
            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void TestReportsStaticLockObject()
        {
            const string test = @"
namespace ParaSmeller.Test.TestCodeTester
{
    public class SynchronizedFinalizer
    {
        public static int Counter;
        private  readonly object LockObjectA = new object();

        public SynchronizedFinalizer()
        {
            lock (LockObjectA)
            {
                Counter++;
            }
        }

        ~SynchronizedFinalizer()
        {
            lock (LockObjectA)
            {
                Counter--;
            }
        }
    }
}";
            var expected = new[] {
                new DiagnosticResult{
                Id = FinalizerReporter.FinalizerSynchronizationDiagnosticId,
                Message = FinalizerReporter.MessageFormatFinalizerSynchronization.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 13, 17)
                        }
                },
                new DiagnosticResult{
                Id = FinalizerReporter.FinalizerSynchronizationDiagnosticId,
                Message = FinalizerReporter.MessageFormatFinalizerSynchronization.ToString(),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 21, 17)
                        }
                }
            };
            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new FinalizerSynchronizationAnalyzer();
        }
    }
}
