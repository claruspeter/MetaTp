// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#I @"build"
#r "metatp"

open MetaTp.Sample
// Define your library scripting code here
type test = Fred.Pizza<"Maybe a connection string here", SomeRandomNumberPerhaps=42>

printfn "%A" test.Address.NAME
printfn "%A" test.Address.Street

let jjj = test.Address.Proxy()
jjj.Street <- "Dinnis Ave"
jjj.Number <- 19
jjj.Street
jjj.Number
jjj
