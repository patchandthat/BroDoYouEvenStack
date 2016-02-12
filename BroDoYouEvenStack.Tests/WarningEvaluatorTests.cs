using System;
using BroDoYouEvenStack.Util;
using Xunit;

namespace BroDoYouEvenStack.Tests
{
    public class WarningEvaluatorTests
    {
        public class IsTimeForWarningTests
        {
            private readonly WarningEvaluator _warning = new WarningEvaluator(new TimeSpan(0, 0, 25), WarningType.RuneSpawn);

            [Fact]
            public void Game_time_0_should_not_warn()
            {
                Assert.False(_warning.IsTimeForWarning(0));
            }

            [Fact]
            public void Game_time_1_35_should_warn()
            {
                Assert.True(_warning.IsTimeForWarning(95));
            }

            [Fact]
            public void Game_time_3_35_should_warn()
            {
                Assert.True(_warning.IsTimeForWarning(215));
            }

            [Fact]
            public void Game_time_2_35_should_not_warn()
            {
                Assert.False(_warning.IsTimeForWarning(155));
            }
        }

        public class ProgressPercentageTests
        {
            private readonly WarningEvaluator _warning = new WarningEvaluator(new TimeSpan(0, 0, 30), WarningType.RuneSpawn);

            [Fact]
            public void Returns_0_percent_when_warning_time_is_up()
            {
                int time = 90;
                const int expected = 0;
                Assert.True(_warning.IsTimeForWarning(time), "Precondition failed, warning is not due");

                int percent = _warning.ProgressPercent(time);

                
                Assert.Equal(expected, percent);
            }

            [Fact]
            public void Returns_25_percent_when_clock_is_zero()
            {
                int time = 0;
                const int expected = 25;

                int percent = _warning.ProgressPercent(time);
                
                Assert.Equal(expected, percent);
            }

            [Fact]
            public void Returns_25_percent_at_two_minutes()
            {
                int time = 120;
                const int expected = 25;

                int percent = _warning.ProgressPercent(time);

                Assert.Equal(expected, percent);
            }

            [Fact]
            public void Returns_50_percent_at_thirty_seconds()
            {
                int time = 30;
                const int expected = 50;

                int percent = _warning.ProgressPercent(time);

                Assert.Equal(expected, percent);
            }

            [Fact]
            public void Returns_50_percent_at_one_minute_thirty_seconds()
            {
                int time = 150;
                const int expected = 50;

                int percent = _warning.ProgressPercent(time);

                Assert.Equal(expected, percent);
            }

            [Fact]
            public void Returns_75_percent_at_one_minute()
            {
                int time = 60;
                const int expected = 75;

                int percent = _warning.ProgressPercent(time);

                Assert.Equal(expected, percent);
            }

            [Fact]
            public void Returns_75_percent_at_three_minutes()
            {
                int time = 180;
                const int expected = 75;

                int percent = _warning.ProgressPercent(time);

                Assert.Equal(expected, percent);
            }

            [Fact]
            public void Returns_99_percent_when_time_almost_up()
            {
                int time = 89;
                const int expected = 99;

                int percent = _warning.ProgressPercent(time);
                
                Assert.Equal(expected, percent);
            }
        }
    }
}
