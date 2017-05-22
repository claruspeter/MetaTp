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

  let dependencyResolve = System.ResolveEventHandler(fun _ args ->
        let asmName = AssemblyName(args.Name)
        let expectedName = asmName.Name + ".dll"
        let packageLocation = Path.Combine( config.ResolutionFolder, "packages", asmName.Name, "lib", "net45", expectedName)
        let localLocation =
            let d = IO.Path.GetDirectoryName(config.RuntimeAssembly)
            IO.Path.Combine(d, expectedName)
        printfn "Attempting to load dependency %A for %A" asmName.Name config.RuntimeAssembly
        match File.Exists localLocation, File.Exists packageLocation with
        | true, _ -> Assembly.LoadFrom localLocation
        | _, true -> Assembly.LoadFrom packageLocation
        | _, _ -> null
      )

  do 
    System.AppDomain.CurrentDomain.add_AssemblyResolve dependencyResolve
  do
    this.AddNamespace(parameters.nameSpace, [Helper.addIncludedType schema])
  do
    schema.DefineStaticParameters( para, buildSchema )
