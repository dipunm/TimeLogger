using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using TimeLogger.Core.Data;
using TimeLogger.Core.OfficeManager;
using TimeLogger.Core.Utils;
using TimeLogger.Domain.OfficeManager;

namespace TimeLogger.Tests.Unit
{
    [TestFixture]
    public class UIConsumerTests
    {
        private Mock<ITimerFactory> _timerFactory;
        private Mock<IClock> _clock;
        private Mock<IWorkRepository> _storage;
        private Mock<IUserTracker> _tracker;
        private OfficeManager _officeManager;
        private Mock<ITimeLoggingConsumer> _consumer;

        [SetUp]
        public void SetUp()
        {
            _timerFactory = new Mock<ITimerFactory>();
            _clock = new Mock<IClock>();
            _storage = new Mock<IWorkRepository>();
            _tracker = new Mock<IUserTracker>();
            _consumer = new Mock<ITimeLoggingConsumer>();
            _officeManager = new OfficeManager(_timerFactory.Object, _clock.Object, _storage.Object, _tracker.Object);
        }

        [Test]
        public void ClockIn_AlreadyClockedIn_()
        {
            _officeManager.ClockIn(_consumer.Object);
            _officeManager.ClockIn(_consumer.Object);
        }
    }
}
