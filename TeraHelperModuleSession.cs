using Celeste.Mod.TeraHelper.DataBase;
using Monocle;

namespace Celeste.Mod.TeraHelper
{
    public class TeraHelperModuleSession : EverestModuleSession
    {
        public bool ActiveTera = false;
        public TeraType StartTera = TeraType.Any;
        public void PrintInfo(string info)
        {
            Engine.Commands.Log($"{info}:\nActive Tera: {ActiveTera}\nStart Tera: {StartTera}");
        }
    }
}
