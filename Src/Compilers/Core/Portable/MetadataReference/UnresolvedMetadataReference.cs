﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Represents a metadata reference that can't be resolved.
    /// </summary>
    /// <remarks>
    /// For error reporting only, can't be used to reference a metadata file.
    /// </remarks>
    public sealed class UnresolvedMetadataReference : MetadataReference
    {
        public string Reference { get; private set; }

        internal UnresolvedMetadataReference(string reference, MetadataReferenceProperties properties)
            : base(properties)
        {
            this.Reference = reference;
        }

        public override string Display
        {
            get
            {
                return CodeAnalysisResources.Unresolved + Reference;
            }
        }

        internal override bool IsUnresolved
        {
            get { return true; }
        }
    }
}