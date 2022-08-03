# Plugin Reloader

Primarily a development tool to speed up experimenting.

For example, it was very useful for me when messing with GUI, I probably reloaded a thousand times per day during development of the [Particle Editor](https://github.com/THDigi/ParticleEditor).


### Usage

Place/symlink your .dll plugins in `%appdata%/SpaceEngineers/DynamicPlugins/` folder only, do not have them be referenced by game too.

To reload the plugins, press `Ctrl+Alt+L` in-game.

**NOTE:** this is a very simple plugin and was not tested with components that get handled by the game (Session, GameLogic, TSS, etc), they won't be unloaded or they might not be loaded in the first place.
