# TNTSim
A Simulator/Calculator for m^3's Orbital Strike Cannon 2.0

## Installation

1. Go to the [Releases](https://github.com/Techcraft7/TNTSim/releases) page and download the `.zip` file for your platform
2. Extract the `.zip` file
3. Run TNTSim

### My platform is not listed!

Create a release build using the instructions below

## Building and Testing

> Requires .NET SDK 7+

⚠ The following commands assume you are in the same folder as `TNTSim.csproj`, otherwise you will need to specify where it is using the `--project` option.

### Release mode
```
dotnet publish -c Release --sc --ucr
```
This create a build for your machine.
If you want to specify an output folder, add `-o <folder name>` to the above command.
If you don't, it will go into the `bin/Release/<.NET version>/<os>-<arch>/publish` folder.

> You can ignore the `.pdb`, `.dbg`, or `.dSYM` files. They are just symbol files that are not needed. You can delete them.

### Debug mode and Testing
```
dotnet build
```
Or, to run the project:
```
dotnet run
```
