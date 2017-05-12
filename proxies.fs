module metatp.proxyHelper

open System
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes
open Microsoft.FSharp.Quotations
open helper

let makeProvidedPrivateReadonlyField fieldName fieldType =
  let field = ProvidedField(fieldName, fieldType)
  field.SetFieldAttributes(FieldAttributes.Private )
  field

let makeInstanceProp (field:ProvidedField) (enclosingType:ProvidedTypeDefinition) =
    let name = field.Name.Substring(1)
    let propType = field.FieldType
    let prop = ProvidedProperty(
                name,
                propType,
                IsStatic = false,
                GetterCode = fun args ->
                    let fieldGet = Expr.FieldGet(args.[0], field)
                    fieldGet
                ,SetterCode = fun args ->
                    let fieldSet = Expr.FieldSet(args.[0], field, args.[1])
                    fieldSet
                )
    prop

let createCtor nameOfEnclosingType =
  let ctor = ProvidedConstructor(
              parameters = [],
              InvokeCode = fun args ->
                  <@@ "" :> obj @@>
              )
  ctor.AddXmlDocDelayed( fun ()-> "Initializes an instance of " + nameOfEnclosingType )
  ctor

let makeProxy (table:MetaTable) proxyName =
  let fields =
    table.columns
    |> Array.map ( fun col -> makeProvidedPrivateReadonlyField ("_" + col.name) col.coltype )

  let proxy =
    makeIncludedType proxyName
    |> addMembers fields
    |> addMember (createCtor table.name)
    //|> addMember (createSimpleToString name)

  let propertythunk (col:ProvidedField) = makeInstanceProp col proxy
  proxy |> addMembers ( fields |> Array.map ( propertythunk ))
