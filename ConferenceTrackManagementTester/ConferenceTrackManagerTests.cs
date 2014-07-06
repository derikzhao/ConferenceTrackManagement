using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using ConferenceTrackManagement;
using NUnit.Framework;

namespace ConferenceTrackManagementTester
{
    [TestFixture]
    public class ConferenceTrackManagerShould
    {
        private Conference _testConference;
        private TalkDuration _duration;
        private IScheduler _scheduler;
        private List<Track> _tracks;

        [SetUp]
        public void Initialize()
        {
            _duration = new TalkDuration(TimeUnit.Min, 60);
            _scheduler=new SimpleScheduler();
            SetTrackSchedule();
            _testConference = new Conference(_scheduler,_tracks);
            _testConference.ResultFormatter = new TextFileFormatter();
        }

        private void SetTrackSchedule()
        {
            _tracks=new List<Track>();
            for (var i = 0; i < 2; i++)
            {
                _tracks.Add(Helper.GetNewTrack());
            }
        }

        
        [TearDown]
        public void CleanUp()
        {
            _testConference = null;
        }

        [Test]
        public void DoNothing()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void ReturnZeroIfNoTalksRegistered()
        {
            Assert.AreEqual(_testConference.TotalTalks, 0);
        }

        [Test]
        public void ReturnOneIfOneTalkRegistered()
        {

            Talk testTalk = new Talk("test Topic", _duration);

            _testConference.TalksLoader = new SingleTalkLoader(testTalk);
            _testConference.RegisterTalks();

            Assert.AreEqual(_testConference.TotalTalks, 1);
        }

        [Test]
        public void RegisterTheTalkProperly()
        {
            Talk testTalk = new Talk("test Topic", _duration);

            _testConference.TalksLoader=new SingleTalkLoader(testTalk);
            _testConference.RegisterTalks();
            Talk registerTestTalk = _testConference.GetTalkByName("test Topic");

            Assert.AreEqual(testTalk.Topic, registerTestTalk.Topic);
            Assert.AreEqual(_testConference.TotalTalks, 1);
            Assert.AreEqual(testTalk.Duration, registerTestTalk.Duration);
        }

        [Test]
        public void RegisterLightningTalks()
        {
            _testConference.TalksLoader = new FileTalksLoader(Helper.GetLightningTalksList());
            _testConference.RegisterTalks();
        }

        [Test]
        public void RegisterMultipleTalksProperly()
        {
            var testTalk1 = new Talk("TopicOne", _duration);
            var testTalk2 = new Talk("TopicTwo", _duration);
            var testTalk3 = new Talk("TopicThree", _duration);

            _testConference.TalksLoader = new SingleTalkLoader(testTalk1);
            _testConference.RegisterTalks();
            _testConference.TalksLoader = new SingleTalkLoader(testTalk2);
            _testConference.RegisterTalks();
            _testConference.TalksLoader = new SingleTalkLoader(testTalk3);
            _testConference.RegisterTalks();
            

            var registeredTalk = _testConference.GetTalkByName("TopicOne");

            Assert.AreEqual(_testConference.TotalTalks, 3);
            Assert.AreEqual(testTalk1.Topic, registeredTalk.Topic);
        }

        [Test]
        public void ReturnNullIfTalkIsNotRegistered()
        {
            Talk testTalk1 = new Talk("TopicOne", _duration);
            _testConference.TalksLoader = new SingleTalkLoader(testTalk1);
            _testConference.RegisterTalks();
            
            var registeredTalk = _testConference.GetTalkByName("TopicTwo");

            Assert.AreEqual(_testConference.TotalTalks, 1);
            Assert.AreEqual(null, registeredTalk);
        }

        [Test]
        [ExpectedException]
        public void NotRegisterTalksIfItCannotBeScheduled()
        {
            _testConference = new Conference(_scheduler, new List<Track>(){
                                                                            Helper.GetNewTrack()
                                                                          });
         
            _testConference.TalksLoader = new FileTalksLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

        }

        [Test]
        public void BeAbleToImportTalksList()
        {
            _testConference.TalksLoader = new FileTalksLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();
            Assert.AreEqual(_testConference.TotalTalks, 13);
        }

        [Test]
        public void AddTalksIfTheyWereAddedInTwoTurns()
        {
            _testConference.TalksLoader = new FileTalksLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

            _testConference.TalksLoader = new FileTalksLoader(Helper.GetTalksListTwo());
            _testConference.RegisterTalks();

            Assert.AreEqual(19, _testConference.TotalTalks);
        }

        [Test]
        public void ScheduleTalks()
        {
            _testConference.TalksLoader = new FileTalksLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

            _testConference.Schedule();
        }

        [Test]
        public void AlsoScheduleTalksWithNetworkingEvent()
        {
            _testConference.TalksLoader = new FileTalksLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

            _testConference.TalksLoader = new FileTalksLoader(Helper.GetTalksListTwo());
            _testConference.RegisterTalks();

            _testConference.Schedule();
        }

        [Test]
        public void ScheduleAllTheTalksRegistered()
        {
            _testConference.TalksLoader = new FileTalksLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

            _testConference.Schedule();

            Assert.AreEqual(13, GetScheduledTalks());
        }

        [Test]
        public void ScheduleIfTalksWereRegisteredInIterations()
        {
            _testConference.TalksLoader = new FileTalksLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

            _testConference.TalksLoader = new FileTalksLoader(Helper.GetTalksListTwo());
            _testConference.RegisterTalks();

            _testConference.Schedule();
            Assert.AreEqual(19, GetScheduledTalks());
        }

        [Test]
        public void WriteResultToTextFile()
        {
            _testConference.TalksLoader = new FileTalksLoader(Helper.GetTalksListOne());
            _testConference.RegisterTalks();

            _testConference.TalksLoader = new FileTalksLoader(Helper.GetTalksListTwo());
            _testConference.RegisterTalks();

            _testConference.Schedule();

            _testConference.GetSchedule();
        }


        private int GetScheduledTalks()
        {
            return _testConference.Tracks.Sum(track => (track.EveningSession.Talks.Count + track.MorningSession.Talks.Count));
        }
    }

    [TestFixture]
    public class TalkShould
    {
        private Talk _testTalk;
        private TalkDuration _duration;
        [SetUp]
        public void Initialize()
        {
            _duration = new TalkDuration(TimeUnit.Min, 60);
            _testTalk = new Talk("Topic", _duration);
        }

        [Test]
        [ExpectedException]
        public void ThrowExceptionIfTitleContainsNumbers()
        {
            _testTalk = new Talk("Topic1", _duration);
        }

        [Test]
        [ExpectedException]
        public void ThrowExceptionForInvalidDuration()
        {
            _duration = new TalkDuration(TimeUnit.Min, -10);
            _testTalk = new Talk("Topic", _duration);
        }

    }

    [TestFixture]
    public class SchedulerShould
    {
        [Test]
        [ExpectedException]
        public void ThrowExceptionWhenNullTracksWerePassed()
        {
            var scheduler = new SimpleScheduler();
            scheduler.Schedule(null,null);
        }
    }
}
