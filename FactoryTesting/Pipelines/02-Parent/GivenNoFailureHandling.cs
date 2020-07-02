using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Threading.Tasks;

namespace FactoryTesting.Pipelines.Parent
{
    public class GivenNoFailureHandling
    {
        private ParentHelper _helper;

        [OneTimeSetUp]
        public async Task WhenPipelineRun()
        {
            _helper = new ParentHelper()
                .WithEmptyTable("procfwk.CurrentExecution")
                .WithEmptyTable("procfwk.ExecutionLog")
                .WithSimulatedError()
                .WithFailureHandling("None");
            await _helper.RunPipeline();
        }

        #region Functional tests

        [Test, Order(1)]
        public void ThenPipelineOutcomeIsFailed()
        {
            _helper.RunOutcome.Should().Be("Failed");
        }
        
        [Test, Order(2)]
        public void ThenTenExecutionsSucceeded()
        {
            _helper.RowCount("procfwk.CurrentExecution", where: "PipelineStatus", equals: "Success").Should().Be(10);
        }
        
        [Test, Order(3)]
        public void ThenOneExecutionFailed()
        {
            _helper.RowCount("procfwk.CurrentExecution", where: "PipelineStatus", equals: "Failed").Should().Be(1);
        }

        [Test, Order(4)]
        public void ThenOneExecutionLogRecord()
        {
            _helper.RowCount("procfwk.ExecutionLog", where: "PipelineStatus", equals: "Failed").Should().Be(1);
        }

        #endregion

        [OneTimeTearDown]
        public void TearDown()
        {
            _helper?.TearDown();
        }
    }
}
