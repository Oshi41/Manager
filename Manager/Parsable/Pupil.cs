using System.Collections.Generic;
using System.Linq;

namespace Manager.Parsable
{
    public class Pupil
    {
        public string Name { get; set; }
        public List<Lesson> Lessons { get; set; } = new List<Lesson>();

        public override bool Equals(object obj)
        {
            if (!(obj is Pupil pupil))
                return false;

            if (!string.Equals(Name, pupil.Name)
                || pupil.Lessons.Count != Lessons.Count)
                return false;

            
            var equals = !Lessons.Except(pupil.Lessons).Any()
                         && !pupil.Lessons.Except(Lessons).Any();

            return equals;
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }
        
        public void Invalidate()
        {
            // Пока что просто расставляем lessons то же имя
            for (var i = Lessons.Count - 1; i >= 0; i--)
            {
                Lessons[i].Name = Name;
            }
        }
    }
}
