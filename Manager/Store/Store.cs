using Manager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Manager.Helper;
using WeakEvent;

namespace Manager.Store
{
    public class Store
    {
        #region Singletone

        private static Store _instance;

        public static Store Instance => _instance ?? (_instance = new Store());

        private Store() { }

        #endregion

        #region Properties

        private readonly List<Pupil> _pupils = new List<Pupil>();

        #endregion

        #region Event
        
        private readonly WeakEventSource<EventArgs> _myEventSource = new WeakEventSource<EventArgs>();

        /// <summary>
        /// Событие изменения списка
        /// </summary>
        public event EventHandler<EventArgs> StoreChanged
        {
            add => _myEventSource.Subscribe(value);
            remove => _myEventSource.Unsubscribe(value);
        }
        
        private void RiseEvent(object o = null)
        {
            _myEventSource.Raise(o, EventArgs.Empty);
        }

        #endregion

        #region Load

        public void Load(IEnumerable<Pupil> pupils)
        {
            _pupils.Clear();

            _pupils.AddRange(pupils);

            RiseEvent(pupils.ToList());
        }

        public void Load(Pupil pupil)
        {
            if (pupil == null)
                return;

            var find = _pupils.FirstOrDefault(x => string.Equals(pupil.Name, x.Name));
            if (find != null)
                _pupils.Remove(find);

            _pupils.Add(pupil);
            RiseEvent(pupil);
        }

        public void RemovePupil(string name)
        {
            if (name == null)
                return;

            var find = _pupils.FirstOrDefault(x => string.Equals(name, x.Name));
            if (find == null)
                return;

            _pupils.Remove(find);
            RiseEvent(name);
        }

        public void ReplacePupil(string old, Pupil pupil)
        {
            if (old == null || _pupils.Contains(pupil))
                return;

            var find = _pupils.FirstOrDefault(x => string.Equals(old, x.Name));
            if (find == null)
                return;

            _pupils.Remove(find);
            _pupils.Add(pupil);

            RiseEvent(pupil);
        }

        public void AddLesson(Lesson lesson)
        {
            if (lesson == null)
                return;

            var find = _pupils.FirstOrDefault(x => string.Equals(lesson.Name, x.Name));
            if (find == null)
                return;

            find.Lessons.Add(lesson);

            RiseEvent(lesson);
        }

        public void RemoveLesson(Lesson lesson)
        {
            if (lesson == null)
                return;

            var find = _pupils.FirstOrDefault(x => string.Equals(lesson.Name, x.Name));
            if (find == null)
                return;

            if (!find.Lessons.Contains(lesson))
                return;

            find.Lessons.Remove(lesson);

            RiseEvent(lesson);
        }

        public void Replace(Lesson old, Lesson lesson)
        {
            if (lesson == null || old == null || !string.Equals(old.Name, lesson.Name))
                return;

            var find = _pupils.FirstOrDefault(x => string.Equals(old.Name, x.Name));
            if (find == null)
                return;

            if (!find.Lessons.Contains(old))
                return;

            find.Lessons.Remove(old);
            find.Lessons.Add(lesson);

            RiseEvent(find);
        }

        #endregion

        #region Find

        public List<Lesson> FindByWeek(DateTime week)
        {
            var lessons = _pupils
                .SelectMany(x => x.Lessons)
                .Where(x => DateHelper.TheSameWeek(x.Date, week))
                .OrderBy(x => x.Date)
                .ToList();

            return lessons;
        }

        public List<Pupil> FindAll()
        {
            return _pupils.ToList();
        }

        public Pupil FindByName(string name)
        {
            if (name == null)
                return null;

            var find = _pupils.FirstOrDefault(x => string.Equals(name, x.Name));

            return find ?? new Pupil();
        }

        #endregion
    }

    
}
