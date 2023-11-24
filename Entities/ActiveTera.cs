using Celeste.Mod.Entities;
using Celeste.Mod.TeraHelper.DataBase;
using Celeste.Mod.TeraHelper.Extensions;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.TeraHelper.Entities;

[CustomEntity("TeraHelper/activeTera")]
public class ActiveTera : Entity
{
    private bool active;
    private TeraType tera;
    private EntityID ID;

    public ActiveTera(EntityData data, Vector2 offset, EntityID id) : base(data.Position + offset)
    {
        ID = id;
        active = data.Bool("active");
        tera = data.Enum("tera", TeraType.Normal);
    }
    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        var session = TeraHelperModule.Session;
        var level = SceneAs<Level>();
        if (session.ActiveTera != active)
        {
            var player = level.Tracker.GetEntity<Player>();
            session.ActiveTera = active;
            if (!active)
            {
                player.RemoveTera();
            }
            else
            {
                if (session.StartTera == TeraType.Any)
                {
                    session.StartTera = tera;
                }
                player.InitTera();
            }
        }
        RemoveSelf();
        level.Session.DoNotLoad.Add(ID);
    }
    public static void OnLoad()
    {
        On.Celeste.Player.ctor += CreatePlayerTera;
        On.Celeste.Session.UpdateLevelStartDashes += UpdateLevelTera;
    }
    public static void OnUnload()
    {
        On.Celeste.Player.ctor -= CreatePlayerTera;
        On.Celeste.Session.UpdateLevelStartDashes -= UpdateLevelTera;
    }
    private static void CreatePlayerTera(On.Celeste.Player.orig_ctor orig, Player self, Vector2 position, PlayerSpriteMode spriteMode)
    {
        if (orig != null)
        {
            orig(self, position, spriteMode);
        }
        if (TeraHelperModule.Session.ActiveTera)
        {
            self.InitTera();
        }
    }
    private static void UpdateLevelTera(On.Celeste.Session.orig_UpdateLevelStartDashes orig, Session self)
    {
        orig(self);
        if (Engine.Scene is not Level level)
            return;
        if (TeraHelperModule.Session.ActiveTera)
        {
            var player = level.Tracker.GetEntity<Player>();
            if (player != null)
            {
                TeraHelperModule.Session.StartTera = player.GetTera(true);
            }
        }
    }
}