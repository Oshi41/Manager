using System.ComponentModel;

namespace Manager.Model
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
