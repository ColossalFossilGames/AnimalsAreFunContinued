# Animals are fun! (Continued)

Spend time and have fun with your pets! This mod lets your pawns play fetch or go for a walk for recreation. Your pawns
will gain a little animal skill as well. This mod is a continuation of the
[Animals are fun mod by Revolus](https://steamcommunity.com/sharedfiles/filedetails/?id=2108362126).

The mod was entirely rewritten and currently only supports versions 1.5 of RimWorld and above. If you are running an older
version of RimWorld, you can download the original mod this was derived from found
[here](https://steamcommunity.com/sharedfiles/filedetails/?id=2108362126).

This mod contains no dependencies and **can be added or removed at any time**. If a save file contains a pawn or animal that
is assigned a job from this mod, you may see an error when loading the save file. However, the error will automatically correct
and will not be displayed on future saves. The source code is automatically bundled when compiling this project and should be
included any copies that are distributed or modified to adhere to the GNU LGPL v2.1 license of the original mod.

Translations are supported for this mod, but should be maintained and distributed in their own mod packages. Both DefInjected
and Keyed translations are supported.

This repository does not include a functioning version of the mod. Instead, you may download it from
[Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3245454244) or follow the Getting Started
instructions below and build your own functioning version of the mod yourself.

If you find any problems, feel free to [file an issue](https://github.com/ColossalFossilGames/AnimalsAreFunContinued/issues)
in the repository.

## Getting Started

This is a Visual Studio project that uses NuGet for package management and a multi-stage build process to generate the entire mod
package. Output of the build process is placed in the `ModPackageFolder` directory.

* The **resources** solution configuration will generate all non-version specific resources.
* The **v1.5bin and v1.6bin** solution configurations will generate the RimWorld version specific assemblies and resources.

When switching between solution configurations, you should wait a few seconds to allow the version specific NuGet resources to be
restored. Otherwise, you may see errors about missing assemblies or resources when trying to build the solution under that
configuration. **Do not** use the batch build feature in Visual Studio to build the solution, as this will not properly restore
the NuGet packages before each build correctly.

To use, copy the AnimalsAreFunContinued folder from the `ModPackageFolder` directory into your local RimWorld mods folder. If the
mod previously existed in your local mods folder, you should delete it first to ensure that only the latest version is used.

Here are some high-level notes of the various files and classes:

- Any user configurable settings should be added to the Settings class and set to be configured in the AnimalsAreFunContinued class
- Logic for JobDriver or JoyGiver for pass/fail checks should be placed in the Validators folder
- Try to keep JobDrivers, JoyGivers and Toils in separate classes

## Built With

- [Krafs.Publicizer](https://github.com/krafs/Publicizer/) - MSBuild plugin for directly accessing non-public members in .NET assemblies.
- [Krafs.RimWorld.Ref](https://github.com/krafs/RimRef/) - Rimworld reference assemblies

## Versioning

[SemVer](http://semver.org/) is used for versioning. For the versions available, see the
[releases on this repository](https://github.com/ColossalFossilGames/AnimalsAreFunContinued/releases).

Major and minor version numbers express the level of compatibility with the corresponding version of RimWorld.

## Authors

- **David LeBlanc** aka **ColossalFossil** - _Initial work_ - [d-leb](https://github.com/d-leb)

See also the list of [contributors](https://github.com/ColossalFossilGames/AnimalsAreFunContinued/graphs/contributors)
who participated in this project.

## License

This project is licensed under the GNU Lesser General Public License version 2.1 - see the [LICENSE](LICENSE)
file for details.