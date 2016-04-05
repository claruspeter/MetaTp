namespace metatp

open System
open System.IO
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes
open helper
open proxyHelper

type MetaProvider(
                    config: TypeProviderConfig,
                    ns:string,
                    typeName:string,
                    tables: obj[] -> MetaTable[]
                  ) as this =
  inherit TypeProviderForNamespaces()
  let asm = (Assembly.LoadFrom(config.RuntimeAssembly))
  let para = ProvidedStaticParameter("A", typeof<string>)
  let schema = helper.makeType asm ns typeName

  let buildSchema =
    fun (typeName:string) (parameterValues: obj[]) ->
        let tableData = parameterValues |> tables
        typeName
          |> makeType asm ns
          |> addMembers (tableData |> Array.map twoLevelProp)
          |> addMember (makeIncludedType "Proxies" |> addMembers (tableData |> Array.map makeProxy))
          |> addIncludedType

  do
    this.AddNamespace(ns, [helper.addIncludedType schema])
  do
    schema.DefineStaticParameters( [para], buildSchema )

[<assembly:TypeProviderAssembly>]
do ()
