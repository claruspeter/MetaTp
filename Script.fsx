// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#I @"build"
#r "metatp"

open MetaTp.Sample
// Define your library scripting code here
type Pizza42 = Fred.Pizza<"Maybe a connection string here", SomeImportantNumberPerhaps=42>

printfn "%A" Pizza42.Address.NAME
printfn "%A" Pizza42.Address.Street

let jjj = Pizza42.Proxies.Address()
jjj.Street <- "Dinnis Ave"
jjj.Number <- 19
jjj.Street
jjj.Number
jjj
