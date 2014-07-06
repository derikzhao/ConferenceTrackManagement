using System;
using System.Collections.Generic;
using ConferenceTrackManagement;

namespace ConferenceTrackManagementTester
{
    public static class Helper
    {
        public static List<string> GetTalksListOne()
        {
            return new List<string>()
            {
                "Accounting-Driven Development 45min",
                "Woah 30min",
                "Sit Down and Write 30min",
                "Pair Programming vs Noise 45min",
                "Rails Magic 60min",
                "Ruby on Rails: Why We Should Move On 60min",
                "Writing Fast Tests Against Enterprise Rails 60min",
                "Clojure Ate Scala (on my project) 45min",
                "Programming in the Boondocks of Seattle 30min",
                "Ruby vs. Clojure for Back-End Development 30min",
                "Ruby on Rails Legacy App Maintenance 60min",
                "A World Without HackerNews 30min",
                "User Interface CSS in Rails Apps 30min"
            };

        }

        public static List<string> GetTalksListTwo()
        {
            return new List<string>()
            {
                "Overdoing it in Python 45min",
                "Lua for the Masses 30min",
                "Ruby Errors from Mismatched Gem Versions 45min",
                "Common Ruby Errors 45min",
                "Rails for Python Developers 30min",
                "Communicating Over Distance 60min"

            };

        }

        public static List<string> GetInvalidTalksList()
        {
            return new List<string>()
            {
                "Overdoing it in Python 45min",
                "Lua for the Masses 30hours"
            };
        }

        public static Track GetNewTrack()
        {
            return new Track()
            {
                MorningSession = new TalkSession()
                {
                    Title = "Morning Session",
                    StartTime = new TimeSpan(09, 00, 00),
                    EndTime = new TimeSpan(12, 00, 00)
                },
                EveningSession = new TalkSession()
                {
                    Title = "Evening Session",
                    StartTime = new TimeSpan(01, 00, 00),
                    EndTime = new TimeSpan(5, 00, 00)
                },
                Networking = new NetworkingEvent()
                {
                    Title = "Networking Event",
                    StartTimeFrom = new TimeSpan(04, 00, 00),
                    StartTimeTo = new TimeSpan(05, 00, 00)
                },
                LunchBreak = new Break()
                {
                    Title = "Lunch Break",
                    StartTime = new TimeSpan(12, 00, 00),
                    EndTime = new TimeSpan(1, 00, 00)
                }
            };
        }

        public static List<string> GetLightningTalksList()
        {
            return new List<string>()
            {
                "Accounting-Driven Development Lightining",
                "Woah lightining"
            };
        }


        public static Talk GetInvalidTalk()
        {
            return new Talk("Topic",new TalkDuration(TimeUnit.Min, 245));
        }
    }
}