namespace metatp

open System
open System.IO
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes

type MetaColumn = {name: string; coltype: Type}
type MetaTable = {name: string; columns: MetaColumn[]}


module helper =


  let provAsm = ProvidedAssembly(Path.ChangeExtension(Path.GetTempFileName(), ".dll"))

  let makeType asm ns typeName =
      ProvidedTypeDefinition( asm, ns, typeName, Some typeof<obj>, IsErased = false)
  let internal makeIncludedType typename =
      ProvidedTypeDefinition(typename, Some typeof<obj>, IsErased=false)
  let addIncludedType (ty:ProvidedTypeDefinition) =
      provAsm.AddTypes([ty])
      ty

  let internal addMember (mi:#MemberInfo) (ty:ProvidedTypeDefinition) =
      ty.AddMember mi
      ty
  let internal addMembers (mi:#MemberInfo[]) (ty:ProvidedTypeDefinition) =
      ty.AddMembersDelayed (fun() -> mi |> Array.toList)
      ty

  let internal createStaticProp (name:string) (value:string) =
      ProvidedProperty(name, typeof<string>, IsStatic = true, GetterCode = (fun args -> <@@ value @@>))
  let internal toStaticProps (vals:string[]) =
      vals |> Array.map (fun a -> createStaticProp a a )

  let asStaticParam nm =
    ProvidedStaticParameter(nm, typeof<string>)

  let twoLevelProp (table:MetaTable) : ProvidedTypeDefinition =
    makeIncludedType table.name
    |> addMember (createStaticProp "NAME" table.name)
    |> addMembers (table.columns|> Array.map (fun t->t.name) |> toStaticProps)


open helper


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
          |> addIncludedType

  do
    this.AddNamespace(ns, [helper.addIncludedType schema])
  do
    schema.DefineStaticParameters( [para], buildSchema )

[<assembly:TypeProviderAssembly>]
do ()
