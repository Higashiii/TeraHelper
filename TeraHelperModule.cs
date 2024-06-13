using Celeste.Mod.TeraHelper.DataBase;
using Celeste.Mod.TeraHelper.Entities;
using System;
using System.Reflection;

namespace Celeste.Mod.TeraHelper
{
    public class TeraHelperModule : EverestModule
    {
        public static TeraHelperModule Instance { get; private set; }

        public override Type SettingsType => typeof(TeraHelperModuleSettings);
        public static TeraHelperModuleSettings Settings => (TeraHelperModuleSettings)Instance._Settings;

        public override Type SessionType => typeof(TeraHelperModuleSession);
        public static TeraHelperModuleSession Session => (TeraHelperModuleSession)Instance._Session;

        public TeraHelperModule()
        {
            Instance = this;
#if DEBUG
            // debug builds use verbose logging
            Logger.SetLogLevel(nameof(TeraHelperModule), LogLevel.Verbose);
#else
            // release builds use info logging to reduce spam in log files
            Logger.SetLogLevel(nameof(TeraHelperModule), LogLevel.Info);
#endif
        }

        public override void Load()
        {
            // TODO: apply any hooks that should always be active
            TeraUtil.OnLoad();
            ActiveTera.OnLoad();
            TeraBlock.OnLoad();
            TeraZipMover.OnLoad();
            TeraFallingBlock.OnLoad();
            TeraBooster.OnLoad();
            TeraDashBlock.OnLoad();
            TeraDreamBlock.OnLoad();
            TeraMoveBlock.OnLoad();
            TeraBarrier.OnLoad();
            TeraSwapBlock.OnLoad();
            TeraCrushBlock.OnLoad();
            TeraBounceBlock.OnLoad();
            TeraCrystal.OnLoad();
        }
        public override void Unload()
        {
            // TODO: unapply any hooks applied in Load()
            TeraUtil.OnUnload();
            ActiveTera.OnUnload();
            TeraBlock.OnUnload();
            TeraZipMover.OnUnload();
            TeraFallingBlock.OnUnload();
            TeraBooster.OnUnload();
            TeraDashBlock.OnUnload();
            TeraDreamBlock.OnUnload();
            TeraMoveBlock.OnUnload();
            TeraBarrier.OnUnload();
            TeraSwapBlock.OnUnload();
            TeraCrushBlock.OnUnload();
            TeraBounceBlock.OnUnload();
            TeraCrystal.OnUnload();
        }
        public static void Debug(string message)
        {
            Logger.Log(nameof(TeraHelperModule), message);
        }
    }
}