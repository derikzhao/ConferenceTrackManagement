using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ConferenceTrackManagement
{
    public class Conference
    {
        public List<Talk> SelectedTalks { get; private set; }

        public ITalksLoader TalksLoader { get; set; }

        public IScheduler Scheduler { get;private set; }

        public IResultFormatter ResultFormatter { get; set; } 

        public List<Track> Tracks { get; private set; }
        
        public int TotalTalks {
            get
            {
                return SelectedTalks.Count;
            }
        }

        private int _remainingTime;

        public Conference(IScheduler scheduler,IEnumerable<Track> tracks)
        {
            Tracks = new List<Track>();
            SelectedTalks = new List<Talk>();

            Tracks = tracks.ToList();
            Scheduler = scheduler;
            CalculateRemainingTime();
        }
        
        public void Schedule()
        {
            Scheduler.Schedule(Tracks,SelectedTalks);
        }
        
        public void GetSchedule()
        {
            ResultFormatter.Format(Tracks);
        }

        public void RegisterTalks()
        {
            
            try
            {
                var newTalks = TalksLoader.Load();
                
                if (CannotBeRegistered(newTalks))
                    throw new ArgumentException("Exceeding Time Limit");
                
                SelectedTalks.InsertRange(SelectedTalks.Count, newTalks);
                
            }
            catch (ArgumentException e)
            {
                throw;
            }
           
        }

        public Talk GetTalkByName(string topic)
        {
            return SelectedTalks.FirstOrDefault(talk => string.Equals(talk.Topic, topic, StringComparison.OrdinalIgnoreCase));
        }

        private bool CannotBeRegistered(IEnumerable<Talk> newTalks)
        {
            var timeTaken = newTalks.Sum(newTalk => newTalk.Duration.Value*(int) newTalk.Duration.Unit);
            if (timeTaken > _remainingTime)
                return true;
            _remainingTime = _remainingTime- timeTaken;
            return false;
        }

        private void CalculateRemainingTime()
        {
            foreach (var track in Tracks)
            {
                _remainingTime += (int)track.MorningSession.EndTime.Subtract(track.MorningSession.StartTime).TotalMinutes;
                _remainingTime += (int)track.EveningSession.EndTime.Subtract(track.EveningSession.StartTime).TotalMinutes;
            }
        }

    }


    public class Track
    {
        public string Title { get; private set; }

        public TalkSession MorningSession { get; set; }

        public Break LunchBreak { get; set ; }

        public TalkSession EveningSession { get; set; }

        public NetworkingEvent Networking { get; set; }
    }
}

