// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake

// Directories
let buildDir  = "./build/"
let deployDir = "./deploy/"
let packagedDir = "./packaged/"


// Filesets
let appReferences  =
    !! "/**/*.csproj"
      ++ "/**/*.fsproj"

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
        |> Log "AppBuild-Output: "
)

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*")
        -- "*.zip"
        |> Zip buildDir (deployDir + "ApplicationName." + version + ".zip")
)

Target "CreatePackage" (fun _ ->
  //CopyFiles buildDir packagedDir

  NuGet (fun p ->
    {p with
      Authors = ["@peterchch"]
      Project = "MetaTp"
      Description = "A meta type provider for creating simple type providers from plain old f# objects"
      OutputPath = packagedDir
      WorkingDir = "."
      Summary = "A meta type provider for creating simple type providers from plain old f# objects"
      Version = "0.2"
      //AccessKey = myAccesskey
      Publish = false
      Files = [(@"build/metatp.dll", Some @"lib/net45", None) ]
      DependenciesByFramework =
        [{
          FrameworkVersion  = "net45"
          Dependencies =
            ["FSharp.TypeProviders.StarterPack", GetPackageVersion "./packages/" "FSharp.TypeProviders.StarterPack"]
        }]
      }
    )
    "metatp.nuspec"
)
// Build order
"Clean"
  ==> "Build"
  ==> "Deploy"

// start build
RunTargetOrDefault "Build"
