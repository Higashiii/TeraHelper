using Celeste.Mod.Helpers;
using Celeste.Mod.TeraHelper.DataBase;
using Celeste.Mod.TeraHelper.Entities;
using Celeste.Mod.TeraHelper.Extensions;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.IO;

namespace Celeste.Mod.TeraHelper.DebugFeatures;

public static class Commands
{
    [Command("changeTera", "Change Player Tera")]
    private static void CmdChangeTera(string teraName)
    {
        if (Engine.Scene is not Level level)
        {
            return;
        }
        if (Enum.TryParse(teraName, out TeraType tera))
        {
            var player = level.Tracker.GetEntity<Player>();
            player.ChangeTera(tera);
        }
    }
}