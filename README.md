# Plugin Reloader
 
This is primarily for developing plugins as I doubt plugins out there expect to be disposed while game is still running or initialized while a world is already running.

It will use %appdata%/SpaceEngineers/DynamicPlugins/ folder to load&reload from.

In-game use Ctrl+Alt+L to trigger a reload of the plugins from that folder.

It does not mess with any plugins loaded by other means (game, plugin loader, etc).