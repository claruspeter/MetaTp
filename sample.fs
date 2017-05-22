module MetaTp.Sample

open System
open MetaTp

let getSchemaFromDataSource (someParameters_InTheSameOrder: obj[]) = 
  match someParameters_InTheSameOrder with
  | [| :? string as connectionstring; :? int as theNumber;|] ->
        [|  //TODO: insert your own magic here to query your data source
            //      and return your types & columns
            {
            name="Address"; 
            columns=
                [|
                {name="Number"; coltype=typeof<int>}
                {name="Street"; coltype=typeof<string>}
                {name="City"; coltype=typeof<string>}
                |] 
            }
        |]
  | _ -> failwith "not the parameters that I expected!!!"

let myProviderParameters =
    {
        nameSpace = "Fred"
        typeName = "Pizza"
        yourTypeParameters = 
          [
            {name="ConnectionStringMaybe"; paratype = typeof<string>}
            {name="SomeImportantNumberPerhaps"; paratype = typeof<int>}
          ]
        schemaFromParameters = getSchemaFromDataSource
    }


[<Microsoft.FSharp.Core.CompilerServices.TypeProvider>]
type MyProvider(config) =
  inherit MetaTp.MetaProvider(config, myProviderParameters )

[<assembly:Microsoft.FSharp.Core.CompilerServices.TypeProviderAssembly>]
do ()
