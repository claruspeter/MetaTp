module MetaTp.Sample

open System
open MetaTp

let getSchemaFromDataSource someParameters_InTheSameOrder = 
    [|
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

let myProviderParameters =
    {
        nameSpace = "Fred"
        typeName = "Pizza"
        yourTypeParameters = 
          [
            {name="ConnectionStringMaybe"; paratype = typeof<string>}
            {name="SomeRandomNumberPerhaps"; paratype = typeof<int>}
          ]
        schemaFromParameters = getSchemaFromDataSource
    }


[<Microsoft.FSharp.Core.CompilerServices.TypeProvider>]
type MyProvider(config) =
  inherit MetaTp.MetaProvider(config, myProviderParameters )

[<assembly:Microsoft.FSharp.Core.CompilerServices.TypeProviderAssembly>]
do ()
