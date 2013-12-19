using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TimeLogger.Cache.Core;
using TimeLogger.Data.Core;
using TimeLogger.Lifecycle.Core;
using TimeLogger.Lifecycle.Domain;
using TimeLogger.Utils.Core;

namespace TimeLogger.Tests.Unit
{
    [TestFixture]
    public class OfficeManagerTests
    {
        private Mock<ITimerFactory> _timerFactory;
        private Mock<IClock> _clock;
        private Mock<ILocalRepository> _storage;
        private OfficeManager _officeManager;
        private Mock<ITimeLoggingConsumer> _consumer;
        private Mock<ITimer> _workTimer;

        private Timings _timings;
        private DateTime _startTime;
        private DateTime _currentTime;

        [SetUp]
        public void SetUp()
        {
            _timerFactory = new Mock<ITimerFactory>();
            _workTimer = new Mock<ITimer>();
            _clock = new Mock<IClock>();
            _storage = new Mock<ILocalRepository>();
            _consumer = new Mock<ITimeLoggingConsumer>();

            _workTimer.SetupAllProperties();
            _timerFactory.Setup(f => f.CreateTimer()).Returns(_workTimer.Object);
            _storage.Setup(s => s.GetLogsForDate(It.IsAny<DateTime>()))
                .Returns(new List<WorkLog>());

            _officeManager = new OfficeManager(_timerFactory.Object, _clock.Object, _storage.Object);

            _timings = new Timings()
                {
                    SleepAmount = TimeSpan.FromMinutes(60),
                    SnoozeAmount = TimeSpan.FromMinutes(3),
                    SnoozeLimit = TimeSpan.FromMinutes(10)
                };
            _consumer.Setup(c => c.GetTimings()).Returns(_timings);
            _consumer.Setup(c => c.GetStartTime()).Returns(() => _startTime);
            _clock.Setup(c => c.Now()).Returns(() => _currentTime);
        }

        [Test]
        public void ClockIn_TimeElapsedSinceReportedStartTime_WorkTimerDurationSetToSleepDurationMinusAlreadyElapsedTime()
        {
            //Arrange
            _timings.SleepAmount = TimeSpan.FromMinutes(60);
            _startTime = new DateTime(2013, 11, 30, 9, 0, 0);
            _currentTime = new DateTime(2013, 11, 30, 9, 3, 20);
            var durationDifference = new TimeSpan(0, 0, 56, 40);
            
            //Act
            _officeManager.ClockIn(_consumer.Object);

            //Assert
            Assert.AreEqual(durationDifference, _workTimer.Object.Duration);
            _workTimer.Verify(t => t.Start());
        }

        [Test]
        public void ClockIn_TimeElapsedSinceReportedStartTimeLongerThanSleepAmount_TimerElapsedEventTriggeredImmediately()
        {
            //Arrange
            _startTime = new DateTime(2013, 11, 30, 9, 0, 0);
            _currentTime = new DateTime(2013, 11, 30, 10, 3, 20);

            //Act
            _officeManager.ClockIn(_consumer.Object);

            //Assert
            _workTimer.Verify(t => t.FireAndReset());
        }

        [Test]
        public void ClockIn_NoTimeElapsedSinceReportedStartTime_WorkTimerDurationSetToSleepDuration()
        {
            //Arrange
            var sleepDuration = TimeSpan.FromMinutes(60);
            _timings.SleepAmount = sleepDuration;
            _startTime = new DateTime(2013, 11, 30, 9, 0, 0);
            _currentTime = new DateTime(2013, 11, 30, 9, 0, 0);

            //Act
            _officeManager.ClockIn(_consumer.Object);

            //Assert
            Assert.AreEqual(sleepDuration, _workTimer.Object.Duration);
            _workTimer.Verify(t => t.Start());
        }

        public void ClockOut_WorkTimerIsInProgress_WorkTimerReset()
        {
         
        }

        public void ClockOut_SnoozeTimerIsInProgress_SnoozeTimerReset()
        {
            
        }

        public void ClockOut_ConsumerHasClockedIn_ConsumerInstructedToDisableSnooze()
        {

        }

        public void ClockOut_MoreThanOneMinuteToLog_ConsumerInstructedToLogTime()
        {
            
        }

        public void ClockOut_ConsumerProvidesEndTime_TimeToLogIsBasedOnProvidedEndTime()
        {

        }

        public void ClockOut_LessThanOneMinuteToLog_ConsumerIsNotInstructedToLogTime()
        {
            
        }

        public void ClockOut_OneMinuteToLog_ConsumerIsInstructedToLogTime()
        {

        }

        public void RemindMeInABit_SleepLimitExceeded_TimerElapsedEventTriggeredImmediately()
        {
            
        }

        public void RemindMeInABit_SleepLimitReached_TimerElapsedEventTriggeredImmediately()
        {

        }

        public void RemindMeInABit_SleepLimitNotExceeded_SleepTimerSetAndStarted()
        {
                
        }

        public void RemindMeInABit_SleepLimitSmallerThanSleepDuration_SleepTimerDurationLessThanSleepAmount()
        {

        }

        public void RemindMeInABit_SleepLimitLargerThanSleepDuration_SleepTimerDurationSetToSleepAmount()
        {

        }

        public void SubmitWork_ListOfWorkProvided_DateOfEachWorkLogSetToCurrentDate()
        {
            
        }

        public void SubmitWork_ListOfWorkProvided_EachWorkLogSentToStorage()
        {

        }

        public void SubmitWork_ListOfWorkProvided_WorkTimerReset()
        {

        }

        public void SubmitWork_TimeToLogIsNotZero_TimerDurationSetToSleepAmountMinusTimeToLog()
        {

        }

        public void SubmitWork_TimeToLogIsNegative_TimerDurationSetToSleepAmountPlusTimeOverlogged()
        {

        }

        public void SubmitWork_TimeToLogIsZero_TimerDurationSetToSleepAmount()
        {

        }

        public void SubmitWork_TimeToLogIsStillLargerThanSleepDuration_TimerElapsedEventTriggeredImmediately()
        {

        }

        public void SubmitWork_ConsumerClockedOut_TimerNotStartedOrFired()
        {

        }

        public void ForceLoggingTime_NoTimeToLog_SleepReset()
        {

        }

        public void ForceLoggingTime_TimeToLog_WorkTimerStopped()
        {

        }

        public void ForceLoggingTime_TimeToLog_ConsumerInstructedToLogCorrectTime()
        {

        }

        public void ForceLoggingTime_NoTimeToLog_ConsumerNotInstructedToLogTime()
        {

        }

        public void ForceLoggingTime_SnoozeTimePlusSleepTimeHasNotYetElapsed_ConsumerInstructedToEnableSnooze()
        {

        }

        public void ForceLoggingTime_SnoozeTimePlusSleepTimeHasNotYetElapsed_SnoozeTimerStarted()
        {

        }

        public void ForceLoggingTime_SnoozeTimePlusSleepTimeHasElapsed_ConsumerInstructedToDisableSnooze()
        {

        }

        public void WorkTimerElapsed_NoTimeToLog_SleepReset()
        {

        }

        public void WorkTimerElapsed_TimeToLog_WorkTimerStopped()
        {

        }

        public void WorkTimerElapsed_TimeToLog_ConsumerInstructedToLogCorrectTime()
        {

        }

        public void WorkTimerElapsed_NoTimeToLog_ConsumerNotInstructedToLogTime()
        {

        }

        public void WorkTimerElapsed_SnoozeTimePlusSleepTimeHasNotYetElapsed_ConsumerInstructedToEnableSnooze()
        {

        }

        public void WorkTimerElapsed_SnoozeTimePlusSleepTimeHasNotYetElapsed_SnoozeTimerStarted()
        {

        }

        public void WorkTimerElapsed_SnoozeTimePlusSleepTimeHasElapsed_ConsumerInstructedToDisableSnooze()
        {

        }

        public void SnoozeTimerElapsed_ConsumerClockedIn_ConsumerInstructedToDisableSleep()
        {
            
        }

        public void SnoozeTimerElapsed_ConsumerClockedOut_ConsumerNotInstructedToDisableSleep()
        {

        }
    }
}
