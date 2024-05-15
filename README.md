# Animals are fun! (Continued)

Spend time and have fun with your pets! This mod lets your pawns play fetch or go for a walk for recreation. Your pawns
will gain a little animal skill as well. This mod is a continuation of the
[Animals are fun mod by Revolus](https://steamcommunity.com/sharedfiles/filedetails/?id=2108362126).

This mod will only support versions 1.5 and forward of RimWorld. If you are running an older version of RimWorld, you 
can download the original mod this was derived from found [here](https://steamcommunity.com/sharedfiles/filedetails/?id=2108362126).

This mod contains no dependencies and **can be added or removed at any time**. This mod does not use the game save file to
store data. The source code is automatically bundled when compiling this project and should be included any copies that
are distributed or modified to adhere to the GNU LGPL v2.1 license of the original mod.

Translations are supported for this mod, but should be maintained and distributed in their own mod packages. Both DefInjected
and Keyed translations are supported.

This repository does not include a functioning version of the mod. Instead, you may download it from
[Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3245454244) or follow the Getting Started
instructions below and build your own functioning version of the mod yourself.

If you find any problems, feel free to [file an issue](https://github.com/ColossalFossilGames/AnimalsAreFunContinued/issues)
in the repository.

## Getting Started

The project uses NuGet for package management. Simply open the solution in Visual Studio 2022 and build the project to
create a usable package. The package will be generated in the ModPackageFolder after a successful build. To use, copy the
AnimalsAreFunContinued folder from ModPackageFolder into your local RimWorld mods folder.

Here are some high-level notes of the various files and classes:

- Any user configurable settings should be added to the Settings class and set to be configured in the AnimalsAreFunContinued class
- Logic for JobDriver or JoyGiver for pass/fail checks should be placed in the EligibilityFlags class
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