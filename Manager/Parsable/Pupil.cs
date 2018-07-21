using System.Collections.Generic;
using System.Linq;

namespace Manager.Model
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
    }
}
