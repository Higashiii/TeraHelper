using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace Celeste.Mod.TeraHelper.DataBase
{
    internal static class TeraUtil
    {
        public static bool IsSuperEffective(TeraType Atk, TeraType Def)
        {
            if (!SuperEffectiveType.ContainsKey(Atk))
                return false;
            var set = SuperEffectiveType[Atk];
            return set.Contains(Def);
        }
        public static bool IsNotEffective(TeraType Atk, TeraType Def)
        {
            if (!NotEffectiveType.ContainsKey(Atk))
                return false;
            var set = NotEffectiveType[Atk];
            return set.Contains(Def);
        }
        public static bool IsNoEffect(TeraType Atk, TeraType Def)
        {
            if (!NoEffectType.ContainsKey(Atk))
                return false;
            var set = NoEffectType[Atk];
            return set.Contains(Def);
        }
        public static TeraEffect GetEffect(TeraType Atk, TeraType Def)
        {
            if (IsSuperEffective(Atk, Def))
                return TeraEffect.Super;
            if (IsNotEffective(Atk, Def))
                return TeraEffect.Bad;
            if (IsNoEffect(Atk, Def))
                return TeraEffect.None;
            return TeraEffect.Normal;
        }
        public static string GetImagePath(TeraType tera)
        {
            return "TeraHelper/objects/tera/Block/" + tera.ToString();
        }
        public static Color GetColor(TeraType tera)
        {
            return tera switch
            {
                TeraType.Bug => new Color(145, 161, 25),
                TeraType.Dragon => new Color(80, 96, 225),
                TeraType.Dark => new Color(80, 65, 63),
                TeraType.Fairy => new Color(239, 112, 239),
                TeraType.Electric => new Color(250, 192, 0),
                TeraType.Ground => new Color(145, 81, 33),
                TeraType.Grass => new Color(63, 161, 41),
                TeraType.Ghost => new Color(112, 65, 112),
                TeraType.Flying => new Color(129, 185, 239),
                TeraType.Fire => new Color(230, 40, 41),
                TeraType.Fighting => new Color(255, 128, 0),
                TeraType.Ice => new Color(63, 216, 255),
                TeraType.Steel => new Color(96, 161, 184),
                TeraType.Rock => new Color(175, 169, 129),
                TeraType.Psychic => new Color(239, 65, 121),
                TeraType.Poison => new Color(145, 65, 203),
                TeraType.Normal => new Color(159, 161, 159),
                TeraType.Water => new Color(41, 128, 239),
                _ => Color.White,
            };
        }
        private static Dictionary<TeraType, HashSet<TeraType>> DefaultSuperEffectiveType = new Dictionary<TeraType, HashSet<TeraType>>();
        private static Dictionary<TeraType, HashSet<TeraType>> DefaultNotEffectiveType = new Dictionary<TeraType, HashSet<TeraType>>();
        private static Dictionary<TeraType, HashSet<TeraType>> DefaultNoEffectType = new Dictionary<TeraType, HashSet<TeraType>>();
        private static Dictionary<TeraType, HashSet<TeraType>> SuperEffectiveType = new Dictionary<TeraType, HashSet<TeraType>>();
        private static Dictionary<TeraType, HashSet<TeraType>> NotEffectiveType = new Dictionary<TeraType, HashSet<TeraType>>();
        private static Dictionary<TeraType, HashSet<TeraType>> NoEffectType = new Dictionary<TeraType, HashSet<TeraType>>();
        private static bool inited = false;
        public static void InitDefaultTeraRelation()
        {
            if (inited)
                return;
            inited = true;
            var path = Path.Combine($"{TeraHelperModule.Instance.Metadata.Name}:", "tera");
            TeraDefine[] define = null;
            Logger.Log(nameof(TeraHelperModule), $"Loading Default Tera Relation From {path}");
            using (FileStream fileStream = FileProxy.OpenRead(path))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    try
                    {
                        if (!reader.EndOfStream)
                        {
                            define = YamlHelper.Deserializer.Deserialize<TeraDefine[]>(reader);
                        }
                    }
                    catch (Exception)
                    {
                        Logger.Log(LogLevel.Warn, nameof(TeraHelperModule), "Failed to load default tera relations");
                    }
                }
            }
            DefaultSuperEffectiveType.Clear();
            DefaultNotEffectiveType.Clear();
            DefaultNoEffectType.Clear();
            if (define != null)
            {
                foreach (var d in define)
                {
                    MakeTeraRelation(d, DefaultSuperEffectiveType, DefaultNotEffectiveType, DefaultNoEffectType);
                }
            }
            //LogTeraRelation(DefaultSuperEffectiveType, DefaultNotEffectiveType, DefaultNoEffectType);
        }
        public static void OnLoad()
        {
            Everest.Events.LevelLoader.OnLoadingThread += LoadTeraRelation;
        }
        public static void OnUnload()
        {
            Everest.Events.LevelLoader.OnLoadingThread -= LoadTeraRelation;
        }
        private static void LoadTeraRelation(Level level)
        {
            SuperEffectiveType.Clear();
            NotEffectiveType.Clear();
            NoEffectType.Clear();
            InitDefaultTeraRelation();
            var path = Path.Combine(Engine.ContentDirectory, "Maps", level.Session.MapData.Filename + ".tera.yaml");
            if (FileProxy.Exists(path))
            {
                Logger.Log(nameof(TeraHelperModule), $"Loading Tera Relation From {path}");
                TeraDefine[] define = null;
                using (FileStream fileStream = FileProxy.OpenRead(path))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        try
                        {
                            if (!reader.EndOfStream)
                            {
                                define = YamlHelper.Deserializer.Deserialize<TeraDefine[]>(reader);
                            }
                        }
                        catch (Exception)
                        {
                            Logger.Log(LogLevel.Warn, nameof(TeraHelperModule), $"Failed to load tera relations at {path}");
                        }
                    }
                }
                if (define == null)
                {
                    SuperEffectiveType.AddRange(DefaultSuperEffectiveType);
                    NotEffectiveType.AddRange(DefaultNotEffectiveType);
                    NoEffectType.AddRange(DefaultNoEffectType);
                }
                else
                {
                    foreach (var d in define)
                    {
                        MakeTeraRelation(d, SuperEffectiveType, NotEffectiveType, NoEffectType);
                    }
                }
            }
            else
            {
                SuperEffectiveType.AddRange(DefaultSuperEffectiveType);
                NotEffectiveType.AddRange(DefaultNotEffectiveType);
                NoEffectType.AddRange(DefaultNoEffectType);
            }
            //LogTeraRelation(SuperEffectiveType, NotEffectiveType, NoEffectType);
        }

        private static void LogTeraRelation(Dictionary<TeraType, HashSet<TeraType>> super, Dictionary<TeraType, HashSet<TeraType>> not, Dictionary<TeraType, HashSet<TeraType>> no)
        {
            foreach (TeraType tera in Enum.GetValues(typeof(TeraType)))
            {
                string str = $"Name: {tera}";
                if (super.ContainsKey(tera))
                {
                    str += $"\nSuperEffective:";
                    foreach (var t in super[tera])
                        str += $"{t} ";
                }
                if (not.ContainsKey(tera))
                {
                    str += $"\nNotEffective:";
                    foreach (var t in not[tera])
                        str += $"{t} ";
                }
                if (no.ContainsKey(tera))
                {
                    str += $"\nNoEffect:";
                    foreach (var t in no[tera])
                        str += $"{t} ";
                }
                Logger.Log(nameof(TeraHelperModule), str);
            }
        }
        private static void MakeTeraRelation(TeraDefine define, Dictionary<TeraType, HashSet<TeraType>> super, Dictionary<TeraType, HashSet<TeraType>> not, Dictionary<TeraType, HashSet<TeraType>> no)
        {
            if (Enum.TryParse(define.Tera, out TeraType tera))
            {
                if (define.SuperEffective != null && define.SuperEffective.Length > 0)
                {
                    if (!super.ContainsKey(tera))
                        super[tera] = new HashSet<TeraType>();
                    foreach(var s in define.SuperEffective)
                    {
                        if (Enum.TryParse(s, out TeraType target))
                        {
                            super[tera].Add(target);
                        }
                    }
                }

                if (define.NotEffective != null && define.NotEffective.Length > 0)
                {
                    if (!not.ContainsKey(tera))
                        not[tera] = new HashSet<TeraType>();
                    foreach (var s in define.NotEffective)
                    {
                        if (Enum.TryParse(s, out TeraType target))
                        {
                            not[tera].Add(target);
                        }
                    }
                }

                if (define.NoEffect != null && define.NoEffect.Length > 0)
                {
                    if (!no.ContainsKey(tera))
                        no[tera] = new HashSet<TeraType>();
                    foreach (var s in define.NoEffect)
                    {
                        if (Enum.TryParse(s, out TeraType target))
                        {
                            no[tera].Add(target);
                        }
                    }
                }
            }
        }
    }
    internal class TeraDefine
    {
        public string Tera = null;
        public string[] SuperEffective = null;
        public string[] NotEffective = null;
        public string[] NoEffect = null;
    }
}
