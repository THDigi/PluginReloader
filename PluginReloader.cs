using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sandbox.ModAPI;
using VRage.FileSystem;
using VRage.Game;
using VRage.Input;
using VRage.Plugins;
using VRage.Utils;

namespace Digi.PluginReloader
{
    public class PluginReloaderPlugin : IPlugin, IHandleInputPlugin
    {
        const string PluginsPath = "DynamicPlugins"; // relative to appdata folder

        object GameInstance;

        readonly List<IPlugin> Plugins = new List<IPlugin>();

        public void Init(object gameInstance)
        {
            GameInstance = gameInstance;

            MyLog.Default.WriteLine("PluginReloader initialized, press Ctrl+Alt+L ingame to reload plugins");
            MyLog.Default.Flush();

            LoadPlugins();
        }

        public void Dispose()
        {
            UnloadPlugins();
            GameInstance = null;
        }

        public void Update()
        {
            try
            {
                if(MyInput.Static == null)
                    return;

                if(MyInput.Static.IsNewKeyPressed(MyKeys.L) && MyInput.Static.IsAnyAltKeyPressed() && MyInput.Static.IsAnyCtrlKeyPressed())
                {
                    UnloadPlugins();
                    LoadPlugins();

                    // I don't really know what I'm doing here xD
                    GC.Collect();
                    GC.WaitForFullGCComplete();
                    GC.Collect();
                }

                foreach(IPlugin plugin in Plugins)
                {
                    try
                    {
                        plugin.Update();
                    }
                    catch(Exception e)
                    {
                        LogError(e);
                    }
                }
            }
            catch(Exception e)
            {
                LogError(e);
            }
        }

        public void HandleInput()
        {
            try
            {
                foreach(IPlugin plugin in Plugins)
                {
                    try
                    {
                        IHandleInputPlugin hip = plugin as IHandleInputPlugin;
                        hip?.HandleInput();
                    }
                    catch(Exception e)
                    {
                        LogError(e);
                    }
                }
            }
            catch(Exception e)
            {
                LogError(e);
            }
        }

        void LoadPlugins()
        {
            string dir = Path.Combine(MyFileSystem.UserDataPath, PluginsPath);

            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                return;
            }

            string[] fileNames = Directory.GetFiles(dir, "*.dll", SearchOption.TopDirectoryOnly);

            foreach(string filePath in fileNames)
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                Assembly assembly = Assembly.Load(bytes);
                CreateInstances(assembly);
            }
        }

        void CreateInstances(Assembly assembly)
        {
            foreach(Type type in assembly.GetTypes())
            {
                try
                {
                    if(type.IsAbstract)
                        continue;

                    if(!type.GetInterfaces().Contains(typeof(IPlugin)))
                        continue;

                    if(type.Name == nameof(PluginReloaderPlugin))
                        continue;

                    IPlugin plugin = (IPlugin)assembly.CreateInstance(type.FullName);
                    plugin.Init(GameInstance);
                    Plugins.Add(plugin);

                    string msg = $"PluginReloader loaded plugin: {plugin}";
                    MyAPIGateway.Utilities?.ShowNotification(msg, 2000, MyFontEnum.Green);
                    MyLog.Default.WriteLine(msg);
                }
                catch(Exception ex)
                {
                    LogError($"Cannot create instance of '{type.FullName}': {ex.ToString()}");
                }
            }
        }

        void UnloadPlugins()
        {
            foreach(IPlugin plugin in Plugins)
            {
                plugin.Dispose();
            }

            Plugins.Clear();
        }

        void LogError(string text)
        {
            string message = $"PluginReloader error: {text}";
            MyLog.Default.WriteLine(message);
            MyAPIGateway.Utilities?.ShowNotification(message, 10000, MyFontEnum.Red);
        }

        void LogError(Exception e)
        {
            MyLog.Default.WriteLine($"PluginReloader error: {e.ToString()}");
            MyAPIGateway.Utilities?.ShowNotification($"PluginReloader error: {e.Message}", 10000, MyFontEnum.Red);
        }
    }
}
