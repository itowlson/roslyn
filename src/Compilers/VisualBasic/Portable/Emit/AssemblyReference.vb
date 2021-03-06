﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Immutable
Imports System.Reflection
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit

    Friend NotInheritable Class AssemblyReference
        Implements Cci.IAssemblyReference

        ' Assembly identity used in metadata to refer to the target assembly.
        ' NOTE: this could be different from assemblySymbol.AssemblyName due to mapping.
        ' For example, multiple assembly symbols might be emitted into a single dynamic assembly whose identity is stored here.
        Public ReadOnly MetadataIdentity As AssemblyIdentity

        Private ReadOnly m_targetAssembly As AssemblySymbol

        Public Sub New(assemblySymbol As AssemblySymbol, symbolMapper As Func(Of AssemblySymbol, AssemblyIdentity))
            Debug.Assert(assemblySymbol IsNot Nothing)
            MetadataIdentity = If(symbolMapper IsNot Nothing, symbolMapper(assemblySymbol), assemblySymbol.Identity)
            m_targetAssembly = assemblySymbol
        End Sub

        Private Sub IReferenceDispatch(visitor As Cci.MetadataVisitor) Implements Cci.IReference.Dispatch
            visitor.Visit(DirectCast(Me, Cci.IAssemblyReference))
        End Sub

        Private ReadOnly Property IAssemblyReferenceCulture As String Implements Cci.IAssemblyReference.Culture
            Get
                Return MetadataIdentity.CultureName
            End Get
        End Property

        Private ReadOnly Property IAssemblyReferenceIsRetargetable As Boolean Implements Cci.IAssemblyReference.IsRetargetable
            Get
                Return MetadataIdentity.IsRetargetable
            End Get
        End Property

        Private ReadOnly Property IAssemblyReferenceContentType As AssemblyContentType Implements Cci.IAssemblyReference.ContentType
            Get
                Return MetadataIdentity.ContentType
            End Get
        End Property

        Private ReadOnly Property IAssemblyReferencePublicKeyToken As ImmutableArray(Of Byte) Implements Cci.IAssemblyReference.PublicKeyToken
            Get
                Return MetadataIdentity.PublicKeyToken
            End Get
        End Property

        Private ReadOnly Property IAssemblyReferenceVersion As Version Implements Cci.IAssemblyReference.Version
            Get
                Return MetadataIdentity.Version
            End Get
        End Property

        Private ReadOnly Property INamedEntityName As String Implements Cci.INamedEntity.Name
            Get
                Return MetadataIdentity.Name
            End Get
        End Property

        Private Function IModuleReferenceGetContainingAssembly(context As EmitContext) As Cci.IAssemblyReference Implements Cci.IModuleReference.GetContainingAssembly
            Return Me
        End Function

        Public Overrides Function ToString() As String
            Return m_targetAssembly.ToString()
        End Function

        Private Function IReferenceAttributes(context As EmitContext) As IEnumerable(Of Cci.ICustomAttribute) Implements Cci.IReference.GetAttributes
            Return SpecializedCollections.EmptyEnumerable(Of Cci.ICustomAttribute)()
        End Function

        Private Function IReferenceAsDefinition(context As EmitContext) As Cci.IDefinition Implements Cci.IReference.AsDefinition
            Return Nothing
        End Function
    End Class
End Namespace
