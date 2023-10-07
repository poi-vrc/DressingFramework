/*
 * Copyright (c) 2023 chocopoi
 * 
 * This file is part of DressingFramework.
 * 
 * DressingFramework is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingFramework. If not, see <https://www.gnu.org/licenses/>.
 */

using Chocopoi.DressingFramework.Logging;
using NUnit.Framework;
using UnityEngine;
using LogType = Chocopoi.DressingFramework.Logging.LogType;

namespace Chocopoi.DressingFramework.Tests.Logging
{
    public class DKReportTest : RuntimeTestBase
    {
        [Test]
        public void GetLogEntriesAsDictionaryTest()
        {
            var report = new DKReport();
            report.LogError("label", "msg1");
            report.LogWarn("label", "msg2");
            report.LogInfo("label", "msg3");
            var dict = report.GetLogEntriesAsDictionary();
            Assert.AreEqual(1, dict[LogType.Error].Count);
            Assert.AreEqual("msg1", dict[LogType.Error][0].message);
            Assert.AreEqual(1, dict[LogType.Warning].Count);
            Assert.AreEqual("msg2", dict[LogType.Warning][0].message);
            Assert.AreEqual(1, dict[LogType.Info].Count);
            Assert.AreEqual("msg3", dict[LogType.Info][0].message);
        }

        [Test]
        public void HasLogCodeTest()
        {
            var report = new DKReport();
            report.LogError("label", "msg", "my-code");
            Assert.True(report.HasLogCode("my-code"));
            Assert.False(report.HasLogCode("not-a-code"));
        }

        [Test]
        public void HasLogCodeByTypeTest()
        {
            var report = new DKReport();
            report.LogError("label", "msg", "my-code");
            Assert.True(report.HasLogCodeByType(LogType.Error, "my-code"));
            Assert.False(report.HasLogCodeByType(LogType.Warning, "my-code"));
        }

        [Test]
        public void HasLogTypeTest()
        {
            var report = new DKReport();
            report.LogError("label", "msg");
            Assert.True(report.HasLogType(LogType.Error));
            Assert.False(report.HasLogType(LogType.Warning));
        }

        [Test]
        public void AppendReportTest()
        {
            var report1 = new DKReport();
            report1.LogError("label1", "hello", "hello-code");
            var report2 = new DKReport();
            report2.LogInfo("label2", "world", "world-code");

            report1.AppendReport(report2);
            Assert.AreEqual(2, report1.LogEntries.Count);
            Assert.True(report1.HasLogCode("hello-code"));
            Assert.True(report1.HasLogCode("world-code"));
        }

        [Test]
        public void LogTest()
        {
            var report = new DKReport();
            report.Log(LogType.Error, "label", "msg", "code1");
            Assert.True(report.HasLogCodeByType(LogType.Error, "code1"));

            report.LogError("label", "msg", "code2");
            Assert.True(report.HasLogCodeByType(LogType.Error, "code2"));

            report.LogInfo("label", "msg", "code3");
            Assert.True(report.HasLogCodeByType(LogType.Info, "code3"));

            report.LogWarn("label", "msg", "code4");
            Assert.True(report.HasLogCodeByType(LogType.Warning, "code4"));

            report.LogDebug("label", "msg", "code5");
            Assert.True(report.HasLogCodeByType(LogType.Debug, "code5"));

            report.LogTrace("label", "msg", "code6");
            Assert.True(report.HasLogCodeByType(LogType.Trace, "code6"));
        }

        public void LogExceptionTest()
        {
            var report = new DKReport();
            report.LogException("label", new System.Exception());
            Assert.AreEqual("System.Exception: Exception of type 'System.Exception' was thrown.", report.LogEntries[0].message);
            report.LogException("label", new System.Exception(), "my-message");
            Assert.AreEqual("my-message: System.Exception: Exception of type 'System.Exception' was thrown.", report.LogEntries[1].message);
        }
    }
}
