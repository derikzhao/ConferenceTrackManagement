using System;
using System.Text.RegularExpressions;

namespace ConferenceTrackManagement
{
    public class Talk
    {
        public string Topic { get; private set; }

        public TalkDuration Duration { get; private set; }

        public Talk(string topic, TalkDuration duration)
        {
            try
            {

                Duration = duration;
                if (IsInValidTitle(topic))
                    throw new ArgumentException("Title Cannot contain Numeric values");
                Topic = topic;
            }
            catch (ArgumentException e)
            {
                throw;
            }

        }

        private bool IsInValidTitle(string title)
        {
            return Regex.IsMatch(title, @"[0-9]+$");
        }

    }

    public class TalkDuration
    {
        public int Value { get; private set; }

        public TimeUnit Unit { get; private set; }

        public TalkDuration(TimeUnit unit, int duration)
        {
            try
            {
                if (IsInvalidDuration(duration))
                    throw new ArgumentException("Invalid Time Value");
                Value = duration;
                Unit = unit;
            }
            catch (ArgumentException e)
            {
                throw;
            }

        }

        private bool IsInvalidDuration(int duration)
        {
            return duration < 0;

        }
    }

    public enum TimeUnit
    {
        Lightining = 5,
        Min = 1
    }

}