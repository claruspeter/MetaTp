namespace MetaTp

open System
open System.IO
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes
open MetaTp.Helper
open MetaTp.Proxies

type MetaParameters =
  {
    nameSpace: string;
    typeName: string;
    yourTypeParameters: MetaParameter list;
    schemaFromParameters: obj[] -> MetaTable[]
  }

type MetaProvider(
                    config: TypeProviderConfig,
                    parameters: MetaParameters
                  ) as this =
  inherit TypeProviderForNamespaces()
  let asm = (Assembly.LoadFrom(config.RuntimeAssembly))
  let para = parameters.yourTypeParameters |> List.map (fun p -> ProvidedStaticParameter(p.name, p.paratype))
  let schema = Helper.makeType asm parameters.nameSpace parameters.typeName

  let propsAndProxy table =
    table 
    |> twoLevelProp
    |> addMember ( makeProxy table "Proxy" )

  let buildSchema =
    fun (typeName:string) (parameterValues: obj[]) ->
        let tableData = parameterValues |> parameters.schemaFromParameters
        typeName
          |> makeType asm parameters.nameSpace
          |> addMembers (tableData |> Array.map propsAndProxy )
          //|> addMember (makeIncludedType "Proxies" |> addMembers (tableData |> Array.map makeProxy))
          |> addIncludedType

  do
    this.AddNamespace(parameters.nameSpace, [Helper.addIncludedType schema])
  do
    schema.DefineStaticParameters( para, buildSchema )
