// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Microsoft.CodeAnalysis.CodeFixes
{
    internal partial class CodeFixService
    {
        private class ProjectCodeFixProvider
        {
            private readonly AnalyzerReference reference;
            private ImmutableDictionary<string, ImmutableArray<CodeFixProvider>> fixersPerLanguage;

            public ProjectCodeFixProvider(AnalyzerReference reference)
            {
                this.reference = reference;
                this.fixersPerLanguage = ImmutableDictionary<string, ImmutableArray<CodeFixProvider>>.Empty;
            }

            public ImmutableArray<CodeFixProvider> GetFixers(string language)
            {
                return ImmutableInterlocked.GetOrAdd(ref this.fixersPerLanguage, language, CreateFixers);
            }

            private ImmutableArray<CodeFixProvider> CreateFixers(string language)
            {
                // check whether the analyzer reference knows how to return fixers directly.
                var codeFixProviderFactory = this.reference as ICodeFixProviderFactory;
                if (codeFixProviderFactory != null)
                {
                    return codeFixProviderFactory.GetFixers();
                }

                // otherwise, see whether we can pick it up from reference itself
                var analyzerFileReference = this.reference as AnalyzerFileReference;
                if (analyzerFileReference == null)
                {
                    return ImmutableArray<CodeFixProvider>.Empty;
                }

                Type[] types = null;
                ImmutableArray<CodeFixProvider>.Builder builder = null;

                try
                {
                    Assembly analyzerAssembly = analyzerFileReference.GetAssembly();
                    types = analyzerAssembly.GetTypes();

                    foreach (var type in types)
                    {
                        if (type.GetTypeInfo().IsSubclassOf(typeof(CodeFixProvider)))
                        {
                            try
                            {
                                var attribute = type.GetCustomAttribute<ExportCodeFixProviderAttribute>();
                                if (attribute != null)
                                {
                                    if (attribute.Languages == null ||
                                        attribute.Languages.Length == 0 ||
                                        attribute.Languages.Contains(language))
                                    {
                                        builder = builder ?? ImmutableArray.CreateBuilder<CodeFixProvider>();
                                        builder.Add((CodeFixProvider)Activator.CreateInstance(type));
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                catch
                {
                    // REVIEW: is the below message right?
                    // NOTE: We could report "unable to load analyzer" exception here but it should have been already reported by DiagnosticService.
                }

                return builder != null ? builder.ToImmutable() : ImmutableArray<CodeFixProvider>.Empty;
            }
        }
    }
}
