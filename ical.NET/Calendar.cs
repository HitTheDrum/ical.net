using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Ical.Net.DataTypes;
using Ical.Net.ExtensionMethods;
using Ical.Net.General.Proxies;
using Ical.Net.Interfaces;
using Ical.Net.Interfaces.Components;
using Ical.Net.Interfaces.DataTypes;
using Ical.Net.Interfaces.Evaluation;
using Ical.Net.Interfaces.General;
using Ical.Net.Interfaces.Serialization;
using Ical.Net.Serialization.iCalendar.Serializers;
using Ical.Net.Structs;

namespace Ical.Net
{
    [Serializable]
    public class Calendar : CalendarComponent, ICalendar, IDisposable
    {
        #region Static Public Methods

        #region LoadFromFile(...)

        #region LoadFromFile(string filepath) variants

        /// <summary>
        /// Loads an <see cref="Calendar"/> from the file system.
        /// </summary>
        /// <param name="filepath">The path to the file to load.</param>
        /// <returns>An <see cref="Calendar"/> object</returns>        
        public static IICalendarCollection LoadFromFile(string filepath)
        {
            return LoadFromFile(filepath, Encoding.UTF8, new CalendarSerializer());
        }

        public static IICalendarCollection LoadFromFile<T>(string filepath) where T : ICalendar
        {
            return LoadFromFile(typeof (T), filepath);
        }

        public static IICalendarCollection LoadFromFile(Type iCalendarType, string filepath)
        {
            ISerializer serializer = new CalendarSerializer();
            serializer.GetService<ISerializationSettings>().CalendarType = iCalendarType;
            return LoadFromFile(filepath, Encoding.UTF8, serializer);
        }

        #endregion

        #region LoadFromFile(string filepath, Encoding encoding) variants

        public static IICalendarCollection LoadFromFile(string filepath, Encoding encoding)
        {
            return LoadFromFile(filepath, encoding, new CalendarSerializer());
        }

        public static IICalendarCollection LoadFromFile<T>(string filepath, Encoding encoding) where T : ICalendar
        {
            return LoadFromFile(typeof (T), filepath, encoding);
        }

        public static IICalendarCollection LoadFromFile(Type iCalendarType, string filepath, Encoding encoding)
        {
            ISerializer serializer = new CalendarSerializer();
            serializer.GetService<ISerializationSettings>().CalendarType = iCalendarType;
            return LoadFromFile(filepath, encoding, serializer);
        }

        public static IICalendarCollection LoadFromFile(string filepath, Encoding encoding, ISerializer serializer)
        {
            // NOTE: Fixes bug #3211934 - Bug in iCalendar.cs - UnauthorizedAccessException
            var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);

            var calendars = LoadFromStream(fs, encoding, serializer);
            fs.Close();
            return calendars;
        }

        #endregion

        #endregion

        #region LoadFromStream(...)

        #region LoadFromStream(Stream s) variants

        /// <summary>
        /// Loads an <see cref="Calendar"/> from an open stream.
        /// </summary>
        /// <param name="s">The stream from which to load the <see cref="Calendar"/> object</param>
        /// <returns>An <see cref="Calendar"/> object</returns>
        public new static IICalendarCollection LoadFromStream(Stream s)
        {
            return LoadFromStream(s, Encoding.UTF8, new CalendarSerializer());
        }

        public static IICalendarCollection LoadFromStream<T>(Stream s) where T : ICalendar
        {
            return LoadFromStream(typeof (T), s);
        }

        public static IICalendarCollection LoadFromStream(Type iCalendarType, Stream s)
        {
            ISerializer serializer = new CalendarSerializer();
            serializer.GetService<ISerializationSettings>().CalendarType = iCalendarType;
            return LoadFromStream(s, Encoding.UTF8, serializer);
        }

        #endregion

        #region LoadFromStream(Stream s, Encoding encoding) variants

        public new static IICalendarCollection LoadFromStream(Stream s, Encoding encoding)
        {
            return LoadFromStream(s, encoding, new CalendarSerializer());
        }

        public new static IICalendarCollection LoadFromStream<T>(Stream s, Encoding encoding) where T : ICalendar
        {
            return LoadFromStream(typeof (T), s, encoding);
        }

        public static IICalendarCollection LoadFromStream(Type iCalendarType, Stream s, Encoding encoding)
        {
            ISerializer serializer = new CalendarSerializer();
            serializer.GetService<ISerializationSettings>().CalendarType = iCalendarType;
            return LoadFromStream(s, encoding, serializer);
        }

        public new static IICalendarCollection LoadFromStream(Stream s, Encoding e, ISerializer serializer)
        {
            return serializer.Deserialize(s, e) as IICalendarCollection;
        }

        #endregion

        #region LoadFromStream(TextReader tr) variants

        public new static IICalendarCollection LoadFromStream(TextReader tr)
        {
            return LoadFromStream(tr, new CalendarSerializer());
        }

        public new static IICalendarCollection LoadFromStream<T>(TextReader tr) where T : ICalendar
        {
            return LoadFromStream(typeof (T), tr);
        }

        public static IICalendarCollection LoadFromStream(Type iCalendarType, TextReader tr)
        {
            ISerializer serializer = new CalendarSerializer();
            serializer.GetService<ISerializationSettings>().CalendarType = iCalendarType;
            return LoadFromStream(tr, serializer);
        }

        public static IICalendarCollection LoadFromStream(TextReader tr, ISerializer serializer)
        {
            var text = tr.ReadToEnd();
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return LoadFromStream(ms, Encoding.UTF8, serializer);
        }

        #endregion

        #endregion

        #region LoadFromUri(...)

        #region LoadFromUri(Uri uri) variants

        /// <summary>
        /// Loads an <see cref="Calendar"/> from a given Uri.
        /// </summary>
        /// <param name="uri">The Uri from which to load the <see cref="Calendar"/> object</param>
        /// <returns>An <see cref="Calendar"/> object</returns>
        public static IICalendarCollection LoadFromUri(Uri uri)
        {
            return LoadFromUri(typeof (Calendar), uri, null, null, null);
        }

        public static IICalendarCollection LoadFromUri<T>(Uri uri) where T : ICalendar
        {
            return LoadFromUri(typeof (T), uri, null, null, null);
        }

        public static IICalendarCollection LoadFromUri(Type iCalendarType, Uri uri)
        {
            return LoadFromUri(iCalendarType, uri, null, null, null);
        }


        public static IICalendarCollection LoadFromUri(Uri uri, WebProxy proxy)
        {
            return LoadFromUri(typeof (Calendar), uri, null, null, proxy);
        }

        public static IICalendarCollection LoadFromUri<T>(Uri uri, WebProxy proxy)
        {
            return LoadFromUri(typeof (T), uri, null, null, proxy);
        }

        public static IICalendarCollection LoadFromUri(Type iCalendarType, Uri uri, WebProxy proxy)
        {
            return LoadFromUri(iCalendarType, uri, null, null, proxy);
        }

        #endregion

        #region LoadFromUri(Uri uri, string username, string password) variants

        /// <summary>
        /// Loads an <see cref="Calendar"/> from a given Uri, using a 
        /// specified <paramref name="username"/> and <paramref name="password"/>
        /// for credentials.
        /// </summary>
        /// <param name="uri">The Uri from which to load the <see cref="Calendar"/> object</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>an <see cref="Calendar"/> object</returns>
        public static IICalendarCollection LoadFromUri(Uri uri, string username, string password)
        {
            return LoadFromUri(typeof (Calendar), uri, username, password, null);
        }

        public static IICalendarCollection LoadFromUri<T>(Uri uri, string username, string password) where T : ICalendar
        {
            return LoadFromUri(typeof (T), uri, username, password, null);
        }

        public static IICalendarCollection LoadFromUri(Type iCalendarType, Uri uri, string username, string password)
        {
            return LoadFromUri(iCalendarType, uri, username, password, null);
        }


        public static IICalendarCollection LoadFromUri(Uri uri, string username, string password, WebProxy proxy)
        {
            return LoadFromUri(typeof (Calendar), uri, username, password, proxy);
        }

        #endregion

        #region LoadFromUri(Type iCalendarType, Uri uri, string username, string password, WebProxy proxy)

        public static IICalendarCollection LoadFromUri(Type iCalendarType, Uri uri, string username, string password, WebProxy proxy)
        {
            try
            {
                var request = WebRequest.Create(uri);

                if (username != null && password != null)
                {
                    request.Credentials = new NetworkCredential(username, password);
                }


                if (proxy != null)
                {
                    request.Proxy = proxy;
                }


                var evt = new AutoResetEvent(false);

                string str = null;
                request.BeginGetResponse(delegate(IAsyncResult result)
                {
                    var e = Encoding.UTF8;

                    try
                    {
                        using (var resp = request.EndGetResponse(result))
                        {
                            // Try to determine the content encoding
                            try
                            {
                                var keys = new List<string>(resp.Headers.AllKeys);
                                if (keys.Contains("Content-Encoding"))
                                {
                                    e = Encoding.GetEncoding(resp.Headers["Content-Encoding"]);
                                }
                            }
                            catch
                            {
                                // Fail gracefully back to UTF-8
                            }

                            using (var stream = resp.GetResponseStream())
                            {
                                using (var sr = new StreamReader(stream, e))
                                {
                                    str = sr.ReadToEnd();
                                }
                            }
                        }
                    }
                    finally
                    {
                        evt.Set();
                    }
                }, null);

                evt.WaitOne();

                if (str != null)
                {
                    return LoadFromStream(new StringReader(str));
                }
                return null;
            }
            catch (WebException)
            {
                return null;
            }
        }

        #endregion

        #endregion

        #endregion

        #region Private Fields

        private IUniqueComponentList<IUniqueComponent> _mUniqueComponents;
        private IUniqueComponentList<IEvent> _mEvents;
        private IUniqueComponentList<ITodo> _mTodos;
        private ICalendarObjectList<IJournal> _mJournals;
        private IUniqueComponentList<IFreeBusy> _mFreeBusy;
        private ICalendarObjectList<ITimeZone> _mTimeZones;

        #endregion

        #region Constructors

        /// <summary>
        /// To load an existing an iCalendar object, use one of the provided LoadFromXXX methods.
        /// <example>
        /// For example, use the following code to load an iCalendar object from a URL:
        /// <code>
        ///     IICalendar iCal = iCalendar.LoadFromUri(new Uri("http://somesite.com/calendar.ics"));
        /// </code>
        /// </example>
        /// </summary>
        public Calendar()
        {
            Initialize();
        }

        private void Initialize()
        {
            Name = Components.Calendar;

            _mUniqueComponents = new UniqueComponentListProxy<IUniqueComponent>(Children);
            _mEvents = new UniqueComponentListProxy<IEvent>(Children);
            _mTodos = new UniqueComponentListProxy<ITodo>(Children);
            _mJournals = new CalendarObjectListProxy<IJournal>(Children);
            _mFreeBusy = new UniqueComponentListProxy<IFreeBusy>(Children);
            _mTimeZones = new CalendarObjectListProxy<ITimeZone>(Children);
        }

        #endregion

        #region Overrides

        protected override void OnDeserializing(StreamingContext context)
        {
            base.OnDeserializing(context);

            Initialize();
        }

        protected bool Equals(Calendar other)
        {
            return base.Equals(other) && Equals(_mUniqueComponents, other._mUniqueComponents) && Equals(_mEvents, other._mEvents) &&
                   Equals(_mTodos, other._mTodos) && Equals(_mJournals, other._mJournals) && Equals(_mFreeBusy, other._mFreeBusy) &&
                   Equals(_mTimeZones, other._mTimeZones);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Calendar) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (_mUniqueComponents != null ? _mUniqueComponents.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_mEvents != null ? _mEvents.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_mTodos != null ? _mTodos.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_mJournals != null ? _mJournals.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_mFreeBusy != null ? _mFreeBusy.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_mTimeZones != null ? _mTimeZones.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion

        #region IICalendar Members

        public virtual IUniqueComponentList<IUniqueComponent> UniqueComponents => _mUniqueComponents;

        public virtual IEnumerable<IRecurrable> RecurringItems
        {
            get
            {
                foreach (object obj in Children)
                {
                    if (obj is IRecurrable)
                    {
                        yield return (IRecurrable) obj;
                    }
                }
            }
        }

        /// <summary>
        /// A collection of <see cref="Components.Event"/> components in the iCalendar.
        /// </summary>
        public virtual IUniqueComponentList<IEvent> Events => _mEvents;

        /// <summary>
        /// A collection of <see cref="Net.FreeBusy"/> components in the iCalendar.
        /// </summary>
        public virtual IUniqueComponentList<IFreeBusy> FreeBusy => _mFreeBusy;

        /// <summary>
        /// A collection of <see cref="Components.Journal"/> components in the iCalendar.
        /// </summary>
        public virtual ICalendarObjectList<IJournal> Journals => _mJournals;

        /// <summary>
        /// A collection of TimeZone components in the iCalendar.
        /// </summary>
        public virtual ICalendarObjectList<ITimeZone> TimeZones => _mTimeZones;

        /// <summary>
        /// A collection of <see cref="Components.Todo"/> components in the iCalendar.
        /// </summary>
        public virtual IUniqueComponentList<ITodo> Todos => _mTodos;

        public virtual string Version
        {
            get { return Properties.Get<string>("VERSION"); }
            set { Properties.Set("VERSION", value); }
        }

        public virtual string ProductId
        {
            get { return Properties.Get<string>("PRODID"); }
            set { Properties.Set("PRODID", value); }
        }

        public virtual string Scale
        {
            get { return Properties.Get<string>("CALSCALE"); }
            set { Properties.Set("CALSCALE", value); }
        }

        public virtual string Method
        {
            get { return Properties.Get<string>("METHOD"); }
            set { Properties.Set("METHOD", value); }
        }

        public virtual RecurrenceRestrictionType RecurrenceRestriction
        {
            get { return Properties.Get<RecurrenceRestrictionType>("X-DDAY-ICAL-RECURRENCE-RESTRICTION"); }
            set { Properties.Set("X-DDAY-ICAL-RECURRENCE-RESTRICTION", value); }
        }

        public virtual RecurrenceEvaluationModeType RecurrenceEvaluationMode
        {
            get { return Properties.Get<RecurrenceEvaluationModeType>("X-DDAY-ICAL-RECURRENCE-EVALUATION-MODE"); }
            set { Properties.Set("X-DDAY-ICAL-RECURRENCE-EVALUATION-MODE", value); }
        }

        /// <summary>
        /// Adds a time zone to the iCalendar.  This time zone may
        /// then be used in date/time objects contained in the 
        /// calendar.
        /// </summary>        
        /// <returns>The time zone added to the calendar.</returns>
        public ITimeZone AddTimeZone(ITimeZone tz)
        {
            this.AddChild(tz);
            return tz;
        }


        /// <summary>
        /// Adds a system time zone to the iCalendar.  This time zone may
        /// then be used in date/time objects contained in the 
        /// calendar.
        /// </summary>
        /// <param name="tzi">A System.TimeZoneInfo object to add to the calendar.</param>
        /// <returns>The time zone added to the calendar.</returns>
        public ITimeZone AddTimeZone(TimeZoneInfo tzi)
        {
            ITimeZone tz = VTimeZone.FromSystemTimeZone(tzi);
            this.AddChild(tz);
            return tz;
        }

        public ITimeZone AddTimeZone(TimeZoneInfo tzi, DateTime earliestDateTimeToSupport, bool includeHistoricalData)
        {
            ITimeZone tz = VTimeZone.FromSystemTimeZone(tzi, earliestDateTimeToSupport, includeHistoricalData);
            this.AddChild(tz);
            return tz;
        }

        /// <summary>
        /// Adds the local system time zone to the iCalendar.  
        /// This time zone may then be used in date/time
        /// objects contained in the calendar.
        /// </summary>
        /// <returns>The time zone added to the calendar.</returns>
        public ITimeZone AddLocalTimeZone()
        {
            ITimeZone tz = VTimeZone.FromLocalTimeZone();
            this.AddChild(tz);
            return tz;
        }

        public ITimeZone AddLocalTimeZone(DateTime earliestDateTimeToSupport, bool includeHistoricalData)
        {
            ITimeZone tz = VTimeZone.FromLocalTimeZone(earliestDateTimeToSupport, includeHistoricalData);
            this.AddChild(tz);
            return tz;
        }


        /// <summary>
        /// Retrieves the TimeZone object for the specified TZID (Time Zone Identifier).
        /// </summary>
        /// <param name="tzId">A valid TZID object, or a valid TZID string.</param>
        /// <returns>A <see cref="TimeZone"/> object for the TZID.</returns>
        public ITimeZone GetTimeZone(string tzId)
        {
            foreach (var tz in TimeZones)
            {
                if (tz.TzId.Equals(tzId))
                {
                    return tz;
                }
            }
            return null;
        }

        /// <summary>
        /// Evaluates component recurrences for the given range of time.
        /// <example>
        ///     For example, if you are displaying a month-view for January 2007,
        ///     you would want to evaluate recurrences for Jan. 1, 2007 to Jan. 31, 2007
        ///     to display relevant information for those dates.
        /// </example>
        /// </summary>
        /// <param name="fromDate">The beginning date/time of the range to test.</param>
        /// <param name="toDate">The end date/time of the range to test.</param>
        [Obsolete("This method is no longer supported.  Use GetOccurrences() instead.")]
        public void Evaluate(IDateTime fromDate, IDateTime toDate)
        {
            throw new NotSupportedException("Evaluate() is no longer supported as a public method.  Use GetOccurrences() instead.");
        }

        /// <summary>
        /// Evaluates component recurrences for the given range of time, for
        /// the type of recurring component specified.
        /// </summary>
        /// <typeparam name="T">The type of component to be evaluated for recurrences.</typeparam>
        /// <param name="fromDate">The beginning date/time of the range to test.</param>
        /// <param name="toDate">The end date/time of the range to test.</param>
        [Obsolete("This method is no longer supported.  Use GetOccurrences() instead.")]
        public void Evaluate<T>(IDateTime fromDate, IDateTime toDate)
        {
            throw new NotSupportedException("Evaluate() is no longer supported as a public method.  Use GetOccurrences() instead.");
        }

        /// <summary>
        /// Clears recurrence evaluations for recurring components.        
        /// </summary>        
        public void ClearEvaluation()
        {
            foreach (var recurrable in RecurringItems)
            {
                recurrable.ClearEvaluation();
            }
        }

        /// <summary>
        /// Returns a list of occurrences of each recurring component
        /// for the date provided (<paramref name="dt"/>).
        /// </summary>
        /// <param name="dt">The date for which to return occurrences. Time is ignored on this parameter.</param>
        /// <returns>A list of occurrences that occur on the given date (<paramref name="dt"/>).</returns>
        public virtual HashSet<Occurrence> GetOccurrences(IDateTime dt)
        {
            return GetOccurrences<IRecurringComponent>(new CalDateTime(dt.AsSystemLocal.Date), new CalDateTime(dt.AsSystemLocal.Date.AddDays(1).AddSeconds(-1)));
        }

        public virtual HashSet<Occurrence> GetOccurrences(DateTime dt)
        {
            return GetOccurrences<IRecurringComponent>(new CalDateTime(dt.Date), new CalDateTime(dt.Date.AddDays(1).AddSeconds(-1)));
        }

        /// <summary>
        /// Returns a list of occurrences of each recurring component
        /// that occur between <paramref name="startTime"/> and <paramref name="endTime"/>.
        /// </summary>
        /// <param name="startTime">The beginning date/time of the range.</param>
        /// <param name="endTime">The end date/time of the range.</param>
        /// <returns>A list of occurrences that fall between the dates provided.</returns>
        public virtual HashSet<Occurrence> GetOccurrences(IDateTime startTime, IDateTime endTime)
        {
            return GetOccurrences<IRecurringComponent>(startTime, endTime);
        }

        public virtual HashSet<Occurrence> GetOccurrences(DateTime startTime, DateTime endTime)
        {
            return GetOccurrences<IRecurringComponent>(new CalDateTime(startTime), new CalDateTime(endTime));
        }

        /// <summary>
        /// Returns all occurrences of components of type T that start on the date provided.
        /// All components starting between 12:00:00AM and 11:59:59 PM will be
        /// returned.
        /// <note>
        /// This will first Evaluate() the date range required in order to
        /// determine the occurrences for the date provided, and then return
        /// the occurrences.
        /// </note>
        /// </summary>
        /// <param name="dt">The date for which to return occurrences.</param>
        /// <returns>A list of Periods representing the occurrences of this object.</returns>
        public virtual HashSet<Occurrence> GetOccurrences<T>(IDateTime dt) where T : IRecurringComponent
        {
            return GetOccurrences<T>(new CalDateTime(dt.AsSystemLocal.Date), new CalDateTime(dt.AsSystemLocal.Date.AddDays(1).AddTicks(-1)));
        }

        public virtual HashSet<Occurrence> GetOccurrences<T>(DateTime dt) where T : IRecurringComponent
        {
            return GetOccurrences<T>(new CalDateTime(dt.Date), new CalDateTime(dt.Date.AddDays(1).AddTicks(-1)));
        }

        /// <summary>
        /// Returns all occurrences of components of type T that start within the date range provided.
        /// All components occurring between <paramref name="startTime"/> and <paramref name="endTime"/>
        /// will be returned.
        /// </summary>
        /// <param name="startTime">The starting date range</param>
        /// <param name="endTime">The ending date range</param>
        public virtual HashSet<Occurrence> GetOccurrences<T>(IDateTime startTime, IDateTime endTime) where T : IRecurringComponent
        {
            var occurrences = new HashSet<Occurrence>();

            occurrences = new HashSet<Occurrence>(RecurringItems.OfType<T>().SelectMany(recurrable => recurrable.GetOccurrences(startTime, endTime)));

            occurrences.ExceptWith(
                occurrences.Where(o => o.Source is IUniqueComponent)
                    .Where(o => o.Source.RecurrenceId != null)
                    .Where(o => o.Source.RecurrenceId.Equals(o.Period.StartTime)));

            return occurrences;
        }

        public virtual HashSet<Occurrence> GetOccurrences<T>(DateTime startTime, DateTime endTime) where T : IRecurringComponent
        {
            return GetOccurrences<T>(new CalDateTime(startTime), new CalDateTime(endTime));
        }

        /// <summary>
        /// Creates a typed object that is a direct child of the iCalendar itself.  Generally,
        /// you would invoke this method to create an Event, Todo, Journal, TimeZone, FreeBusy,
        /// or other base component type.
        /// </summary>
        /// <example>
        /// To create an event, use the following:
        /// <code>
        /// IICalendar iCal = new iCalendar();
        /// 
        /// Event evt = iCal.Create&lt;Event&gt;();
        /// </code>
        /// 
        /// This creates the event, and adds it to the Events list of the iCalendar.
        /// </example>
        /// <typeparam name="T">The type of object to create</typeparam>
        /// <returns>An object of the type specified</returns>
        public T Create<T>() where T : ICalendarComponent
        {
            var obj = Activator.CreateInstance(typeof (T)) as ICalendarObject;
            if (obj is T)
            {
                this.AddChild(obj);
                return (T) obj;
            }
            return default(T);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Children.Clear();
        }

        #endregion

        #region IMergeable Members

        public virtual void MergeWith(IMergeable obj)
        {
            var c = obj as ICalendar;
            if (c != null)
            {
                if (Name == null)
                {
                    Name = c.Name;
                }

                Method = c.Method;
                Version = c.Version;
                ProductId = c.ProductId;
                Scale = c.Scale;

                foreach (var p in c.Properties)
                {
                    if (!Properties.ContainsKey(p.Name))
                    {
                        Properties.Add(p.Copy<ICalendarProperty>());
                    }
                }
                foreach (var child in c.Children)
                {
                    if (child is IUniqueComponent)
                    {
                        if (!UniqueComponents.ContainsKey(((IUniqueComponent) child).Uid))
                        {
                            this.AddChild(child.Copy<ICalendarObject>());
                        }
                    }
                    else if (child is ITimeZone)
                    {
                        var tz = GetTimeZone(((ITimeZone) child).TzId);
                        if (tz == null)
                        {
                            this.AddChild(child.Copy<ICalendarObject>());
                        }
                    }
                    else
                    {
                        this.AddChild(child.Copy<ICalendarObject>());
                    }
                }
            }
        }

        #endregion

        #region IGetFreeBusy Members

        public virtual IFreeBusy GetFreeBusy(IFreeBusy freeBusyRequest)
        {
            return Net.FreeBusy.Create(this, freeBusyRequest);
        }

        public virtual IFreeBusy GetFreeBusy(IDateTime fromInclusive, IDateTime toExclusive)
        {
            return Net.FreeBusy.Create(this, Net.FreeBusy.CreateRequest(fromInclusive, toExclusive, null, null));
        }

        public virtual IFreeBusy GetFreeBusy(IOrganizer organizer, IAttendee[] contacts, IDateTime fromInclusive, IDateTime toExclusive)
        {
            return Net.FreeBusy.Create(this, Net.FreeBusy.CreateRequest(fromInclusive, toExclusive, organizer, contacts));
        }

        #endregion
    }
}