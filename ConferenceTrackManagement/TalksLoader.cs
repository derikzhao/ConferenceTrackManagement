using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ConferenceTrackManagement
{
    public interface ITalksLoader
    {
        IEnumerable<Talk> Load();
    }
    
    public class FileTalksLoader : ITalksLoader
    {
        public List<string> TalksList { get; private set; }

        public FileTalksLoader(List<string> talkList)
        {
            TalksList = new List<string>();
            TalksList = talkList;
        }

        public IEnumerable<Talk> Load()
        {
            var registeredTalks = new List<Talk>();

            foreach (var talk in TalksList)
            {
                var unit = CheckForValidUnit(talk);
                string topic=null;
                

                var durationPosition = talk.IndexOfAny("0123456789".ToCharArray());

                var duration = "1";
                
                if (durationPosition == -1)
                    topic = talk.Substring(0, talk.LastIndexOf(unit, StringComparison.OrdinalIgnoreCase));
                else
                {
                    topic = talk.Substring(0, durationPosition);
                    
                    duration = talk.Substring(durationPosition,
                             talk.LastIndexOfAny("0123456789".ToCharArray()) - durationPosition + 1);
                }
                

                var talkDuration = new TalkDuration((TimeUnit)Enum.Parse(typeof(TimeUnit), unit, true)
                                                    ,Convert.ToInt32(duration));

                registeredTalks.Add(new Talk(topic, talkDuration));
            }
            
            return registeredTalks;
        }

        private string CheckForValidUnit(string talk)
        {
            foreach (var unit in Enum.GetValues(typeof (TimeUnit)))
            {
                if (talk.IndexOf(unit.ToString(), StringComparison.OrdinalIgnoreCase) > -1)
                    return unit.ToString();
            }
            return null;
        }
    }

    public class SingleTalkLoader : ITalksLoader
    {
        private Talk _talk;

        public SingleTalkLoader(Talk talk)
        {
            _talk = talk;
        }

        public IEnumerable<Talk> Load()
        {
            return new List<Talk>(){
                _talk
            };
        }
    }
}
