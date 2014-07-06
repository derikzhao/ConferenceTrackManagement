using System;
using System.Collections.Generic;
using System.Linq;

namespace ConferenceTrackManagement
{
    public interface IScheduler
    {
        void Schedule(IEnumerable<Track> tracks, IEnumerable<Talk> talks);
    }


    public class SimpleScheduler : IScheduler
    {
        List<Track> _tracks;
        private List<Talk> _talks;

        public SimpleScheduler()
        {
            _tracks = new List<Track>();
            _talks = new List<Talk>();
        }

        public void Schedule(IEnumerable<Track> tracks, IEnumerable<Talk> talks)
        {
            _tracks = tracks.ToList();
            _talks = talks.ToList();

            SortTalks();
            InitializeTracks();

            foreach (var talk in _talks)
            {
                var isScheduledInMorning = ScheduleInMorning(talk);

                if (!isScheduledInMorning)
                {
                    ScheduleInEvening(talk);
                }
            }

            ScheduleNetworkingEvent();
        }

        private void ScheduleNetworkingEvent()
        {
            foreach (var track in _tracks)
                track.Networking.StartTime = track.EveningSession.EndTime.Subtract(track.EveningSession.TimeRemaining);
        }

        private void InitializeTracks()
        {
            foreach (var track in _tracks)
            {
                track.MorningSession.Talks=new List<Talk>();
                track.MorningSession.TimeRemaining = track.MorningSession.EndTime.Subtract(track.MorningSession.StartTime);
                
                track.EveningSession.Talks=new List<Talk>();
                track.EveningSession.TimeRemaining = track.EveningSession.EndTime.Subtract(track.EveningSession.StartTime);
            }
        }

        private bool ScheduleInMorning(Talk talk)
        {
            foreach (var track in _tracks)
                {
                    var duration = talk.Duration.Value * (int)(talk.Duration.Unit);
                    if (TalkCanBeScheduledInMorning(duration, track))
                    {
                        track.MorningSession.Talks.Add(talk);
                        track.MorningSession.TimeRemaining=track.MorningSession
                                                                .TimeRemaining.Subtract(new TimeSpan(0, duration, 0));
                        return true;
                    }
                }
            return false;
        }

        private bool TalkCanBeScheduledInMorning(int duration, Track track)
        {
            return (duration <= track.MorningSession.TimeRemaining.TotalMinutes);
        }

        private bool ScheduleInEvening(Talk talk)
        {
            foreach (var track in _tracks)
            {
                var duration = talk.Duration.Value * (int)(talk.Duration.Unit);
                if (TalkCanBeScheduledInEvening(duration, track))
                {
                    track.EveningSession.Talks.Add(talk);
                    track.EveningSession.TimeRemaining = track.EveningSession
                                                              .TimeRemaining.Subtract(new TimeSpan(0, duration, 0));
                    return true;
                }
            }
            return false;
        }

        private bool TalkCanBeScheduledInEvening(int duration, Track track)
        {
            return (duration <= track.EveningSession.TimeRemaining.TotalMinutes);
        }

        private void SortTalks()
        {
            _talks =_talks.OrderByDescending(t => (t.Duration.Value * (int)(t.Duration.Unit))).ToList();
        }


    }
}