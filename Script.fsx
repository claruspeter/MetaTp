// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#I @"build"
#r "metatp"

open metatp
open metatp.sample
// Define your library scripting code here
type test = Fred.Pizza<"q">

printfn "%A" test.Address.NAME
printfn "%A" test.Address.Street
