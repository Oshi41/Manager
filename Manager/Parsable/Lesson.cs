using System;

namespace Manager.Model
{
    public class Lesson
    {
        public string Name { get; set; }
        public string Partner { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public bool IsMain { get; set; } = true;
        public LessonTypes LessonType { get; set; }

        public bool IsVilid()
        {
            if (string.Equals(Name, Partner))
                return false;

            if (Number < 1 || Number > 53)
                return false;

            if (LessonType == LessonTypes.Reading
                && !IsMain)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Number;
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ IsMain.GetHashCode();
                hashCode = (hashCode * 397) ^ LessonType.GetHashCode();
                hashCode = (hashCode * 397) ^ Name?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Partner?.GetHashCode() ?? 0;

                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Lesson lesson))
                return false;

            return string.Equals(Name, lesson.Name) 
                && string.Equals(Partner, lesson.Partner) 
                && Number == lesson.Number 
                && Date == lesson.Date 
                && IsMain == lesson.IsMain
                && LessonType == lesson.LessonType;

        }
    }
}
