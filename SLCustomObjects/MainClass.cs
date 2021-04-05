using Exiled.API.Features;
using Mirror;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SLCustomObjects
{
    public class MainClass : Plugin<PluginConfig>
    {
        public override string Author { get; } = "Killers0992";
        public override string Name { get; } = "SLCustomObjects";
        public override string Prefix { get; } = "slcustomobjects";

        public string pluginDir;
        public Schematic schem;
        public override void OnEnabled()
        {
            schem = new Schematic();
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            pluginDir = Path.Combine(folderPath, "EXILED", "Plugins", "SLCustomObjects");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);
            if (!Directory.Exists(Path.Combine(pluginDir, "schematics")))
                Directory.CreateDirectory(Path.Combine(pluginDir, "schematics"));
            base.OnEnabled();
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += Server_SendingRemoteAdminCommand;
            Exiled.Events.Handlers.Player.PickingUpItem += schem.PickupItem;
        }

        private void Server_SendingRemoteAdminCommand(Exiled.Events.EventArgs.SendingRemoteAdminCommandEventArgs ev)
        {
            switch (ev.Name.ToUpper())
            {
                case "SCHEMATIC":
                    ev.IsAllowed = false;
                    if (ev.Arguments.Count != 0)
                    {
                        switch (ev.Arguments[0].ToUpper())
                        {
                            case "LIST":
                                string outstr = " Schematics: \n";
                                foreach (var file in Directory.GetFiles(Path.Combine(pluginDir, "schematics")))
                                {
                                    outstr += Path.GetFileNameWithoutExtension(file) + "\n";
                                }
                                ev.Sender.RemoteAdminMessage(outstr, true, "SCHEMATIC");
                                break;
                            case "LOAD":
                                if (File.Exists(Path.Combine(pluginDir, "schematics", "schematic-" + ev.Arguments[1] + ".json")))
                                {
                                    if (Schematic.LoadSchematic(Path.Combine(pluginDir, "schematics", "schematic-" + ev.Arguments[1] + ".json"), ev.Sender.Position))
                                    {
                                        ev.Sender.RemoteAdminMessage($"Schematic {ev.Arguments[1]} loaded.", true, "SCHEMATIC");
                                        return;
                                    }
                                    else
                                    {
                                        ev.Sender.RemoteAdminMessage($"Failed loading schematic {ev.Arguments[1]}.", true, "SCHEMATIC");
                                    }
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage("File not found", true, "SCHEMATIC");
                                }
                                break;
                            case "UNLOAD":
                                if (Schematic.UnloadSchematic("Schematic_" + ev.Arguments[1]))
                                {
                                    ev.Sender.RemoteAdminMessage($"Schematic {ev.Arguments[1]} unloaded.", true, "SCHEMATIC");
                                    return;
                                }
                                ev.Sender.RemoteAdminMessage($"Schematic {ev.Arguments[1]} is not loaded.", true, "SCHEMATIC");
                                break;
                            case "BRINGSCHEMATIC":
                                Schematic.BringSchematic("Schematic_"+ev.Arguments[1], ev.Sender);
                                break;
                        }
                    }
                    break;
            }
        }


    }
}
