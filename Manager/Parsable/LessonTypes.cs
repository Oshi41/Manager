using System.ComponentModel;

namespace Manager.Parsable
{
    public enum LessonTypes
    {
        [Description("Чтение")]
        Reading,

        [Description("Первое посещение")]
        First,

        [Description("Повтор")]
        Second,

        [Description("Изучение")]
        Study
    }
}
