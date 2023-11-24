﻿using Celeste.Mod.Entities;
using Celeste.Mod.TeraHelper.DataBase;
using Celeste.Mod.TeraHelper.Extensions;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.TeraHelper.Entities
{
    [TrackedAs(typeof(DashBlock))]
    [Tracked(false)]
    [CustomEntity("TeraHelper/teraDashBlock")]
    public class TeraDashBlock : DashBlock, ITeraBlock
    {
        public TeraType tera { get; set; }
        private Image image;
        public TeraDashBlock(EntityData data, Vector2 offset, EntityID id)
            : base(data.Position + offset, data.Char("tiletype", '3'), data.Width, data.Height, data.Bool("blendin"), data.Bool("permanent", defaultValue: true), data.Bool("canDash", defaultValue: true), id)
        {
            tera = data.Enum("tera", TeraType.Normal);
        }
        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            Add(image = new Image(GFX.Game[TeraUtil.GetImagePath(tera)]));
            image.CenterOrigin();
            image.Position = new Vector2(Width / 2, Height / 2);
        }
        public static void OnLoad()
        {
            On.Celeste.DashBlock.OnDashed += OnTeraDashed;
            IL.Celeste.Platform.MoveHExactCollideSolids += SolidCheckH;
            IL.Celeste.Platform.MoveVExactCollideSolids += SolidCheckV;
        }
        public static void OnUnload()
        {
            On.Celeste.DashBlock.OnDashed -= OnTeraDashed;
            IL.Celeste.Platform.MoveHExactCollideSolids -= SolidCheckH;
            IL.Celeste.Platform.MoveVExactCollideSolids -= SolidCheckV;
        }

        private static DashCollisionResults OnTeraDashed(On.Celeste.DashBlock.orig_OnDashed orig, DashBlock self, Player player, Vector2 direction)
        {
            if (self is TeraDashBlock teraDash)
            {
                if (teraDash != null)
                {
                    var effect = teraDash.EffectAsDefender(player.GetTera());
                    if (effect != TeraEffect.Super)
                    {
                        if (player.StateMachine.State == 10)
                            player.StateMachine.State = 0;
                        return DashCollisionResults.NormalOverride;
                    }
                }
            }
            return orig(self, player, direction);
        }
        private static void SolidCheckH(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCall<Entity>("CollideCheck")))
            {
                ILLabel label = null;
                cursor.GotoNext(MoveType.After, instr => instr.MatchBrfalse(out label));
                var current = cursor.Index;
                if (cursor.TryGotoPrev(MoveType.After, instr => instr.OpCode == OpCodes.Stloc_S && (((VariableDefinition)instr.Operand).Index == 5)))
                {
                    VariableDefinition variable = (VariableDefinition)cursor.Prev.Operand;
                    Logger.Log(nameof(TeraHelperModule), $"Injecting code to apply tera effect on kevin crush dash block H at {cursor.Index} in IL for {cursor.Method.Name}");
                    cursor.Goto(current);
                    cursor.Emit(OpCodes.Ldloc_S, variable);
                    cursor.Emit(OpCodes.Ldarg_0);
                    cursor.EmitDelegate(CrushDash);
                    cursor.Emit(OpCodes.Brfalse, label);
                }
            }
        }
        private static void SolidCheckV(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCall<Entity>("CollideCheck")))
            {
                ILLabel label = null;
                cursor.GotoNext(MoveType.After, instr => instr.MatchBrfalse(out label));
                var current = cursor.Index;
                if (cursor.TryGotoPrev(MoveType.After, instr => instr.OpCode == OpCodes.Stloc_S && (((VariableDefinition)instr.Operand).Index == 5)))
                {
                    VariableDefinition variable = (VariableDefinition)cursor.Prev.Operand;
                    Logger.Log(nameof(TeraHelperModule), $"Injecting code to apply tera effect on kevin crush dash block V at {cursor.Index} in IL for {cursor.Method.Name}");
                    cursor.Goto(current);
                    cursor.Emit(OpCodes.Ldloc_S, variable);
                    cursor.Emit(OpCodes.Ldarg_0);
                    cursor.EmitDelegate(CrushDash);
                    cursor.Emit(OpCodes.Brfalse, label);
                }
            }
        }
        private static bool CrushDash(DashBlock dash, Platform crush)
        {
            if (dash is TeraDashBlock teraDash)
            {
                if (crush is ITeraBlock teraCrush)
                {
                    var effect = teraDash.EffectAsDefender(teraCrush.tera);
                    return effect == TeraEffect.Super;
                }
                return false;
            }
            return true;
        }
        public TeraEffect EffectAsAttacker(TeraType t)
        {
            return TeraUtil.GetEffect(tera, t);
        }

        public TeraEffect EffectAsDefender(TeraType t)
        {
            return TeraUtil.GetEffect(t, tera);
        }
    }
}
