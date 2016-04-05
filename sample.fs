module metatp.sample

open System
open metatp
let c1={name="Number"; coltype=typeof<int>}
let c2={name="Street"; coltype=typeof<string>}
let c3={name="City"; coltype=typeof<string>}
let tables = [|{name="Address"; columns=[|c1;c2;c3|] }|]

[<Microsoft.FSharp.Core.CompilerServices.TypeProvider>]
type MyProvider(config) =
  inherit metatp.MetaProvider(config, "Fred","Pizza", (fun x ->  tables) )
