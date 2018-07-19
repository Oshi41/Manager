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

            var exclude = Lessons.Except(pupil.Lessons);
            return !exclude.Any();
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }
    }
}
