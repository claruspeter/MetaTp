# Meta-TP

A type provider provider.

## Why?

Most type providers I have created (and used) seem to follow a simple pattern that provides

* A schema of entity type name, and their associated property names.
* A set of proxies that follow the above type shape, for instantiation.

## How to Use

You may use this library to create __your__ type provider, by filling in a structure that nominates the type's name and a table of type names and properties
(from your own special data source).

Points to note:

1. You can specify your own type parameters, like connection string perhaps.
1. Getting the schema at use-time is a "callback function", which you will need to provide (this is _your_ magic!) that passes these type parameters' values in an ordered object array.
1. Only a very little amout of boilerplate required at the bottom.

[from sample.fs](sample.fs)
````` fsharp
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
        schemaFromParameters = getSchemaFromDataSource  //callback to get type definitions
    }

// A bit of necessary boilerplate
[<Microsoft.FSharp.Core.CompilerServices.TypeProvider>]
type MyProvider(config) =
    inherit MetaTp.MetaProvider(config, myProviderParameters )

[<assembly:Microsoft.FSharp.Core.CompilerServices.TypeProviderAssembly>]
    do ()

`````

## How to use the type provider that you have created

The user can now include _your_ finished type provider library, and constructs a type based on _your_
type provider including any type parameters you may have specified.

At this time the "callback" is called and your personalized code connects to your 
data source and produces the schema to code against.

[from Script.fsx](Script.fsx)
```` fsharp

open MetaTp.Sample
// Define your library scripting code here
type Pizza42 = Fred.Pizza<"Maybe a connection string", SomeImportantNumberPerhaps=42>

printfn "%A" Pizza42.Address.NAME    //Auto generated member that equals "Address"
printfn "%A" Pizza42.Address.Street  //String static prop with value "Street"

let jjj = Pizza42.Proxies.Address()    //A type with default constructor
                                       // and properties Street,...

````

## Working Example

An working OSS project that uses this library can be found at <https://github.com/claruspeter/neo4jtypeprovider>.

# Licence

This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
