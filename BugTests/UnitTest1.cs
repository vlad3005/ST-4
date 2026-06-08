using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;
using System;
using System.Linq;

namespace BugTests
{
    [TestClass]
    public class DefectWorkflowTests
    {
        private Bug _issueTracker = null!;

        [TestInitialize]
        public void Setup()
        {
            _issueTracker = new Bug();
        }

        [TestMethod]
        public void Verify_InitialState_IsNewDefect()
        {
            Assert.AreEqual(State.NewDefect, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void ApplyTrigger_StartAnalysis_TransitionsToDefectAnalysis()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            Assert.AreEqual(State.DefectAnalysis, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void DefectAnalysis_SetsAssigneeToProductTeam()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            Assert.AreEqual("Product Team", _issueTracker.Assignee);
        }

        [TestMethod]
        public void Triage_MarkAsNotDefect_GoesToReturned()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.MarkAsNotDefect);
            Assert.AreEqual(State.Returned, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void Triage_MarkAsDuplicate_GoesToReturned()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.MarkAsDuplicate);
            Assert.AreEqual(State.Returned, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void Triage_MarkAsWontFix_GoesToReturned()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.MarkAsWontFix);
            Assert.AreEqual(State.Returned, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void Triage_PostponeForLater_GoesToDeferred()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.PostponeForLater);
            Assert.AreEqual(State.Deferred, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void Deferred_ResumeAnalysis_ReturnsToAnalysis()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.PostponeForLater);
            _issueTracker.ApplyTrigger(Trigger.ResumeAnalysis);
            Assert.AreEqual(State.DefectAnalysis, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void Triage_RequestMoreInfo_GoesToNeedMoreInfo()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.RequestMoreInfo);
            Assert.AreEqual(State.NeedMoreInfo, _issueTracker.CurrentState);
            Assert.AreEqual("Tester", _issueTracker.Assignee);
        }

        [TestMethod]
        public void NeedMoreInfo_ProvideInfo_ReturnsToAnalysis()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.RequestMoreInfo);
            _issueTracker.ApplyTrigger(Trigger.ProvideInfo);
            Assert.AreEqual(State.DefectAnalysis, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void Triage_AcceptForFix_GoesToFixingInProgress()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.AcceptForFix);
            Assert.AreEqual(State.FixingInProgress, _issueTracker.CurrentState);
            Assert.AreEqual("Developer", _issueTracker.Assignee);
        }

        [TestMethod]
        public void Fixing_CannotReproduce_GoesToNotReproducible()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.AcceptForFix);
            _issueTracker.ApplyTrigger(Trigger.CannotReproduce);
            Assert.AreEqual(State.NotReproducible, _issueTracker.CurrentState);
            Assert.AreEqual("Tester", _issueTracker.Assignee);
        }

        [TestMethod]
        public void NotReproducible_Confirm_GoesToClosed()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.AcceptForFix);
            _issueTracker.ApplyTrigger(Trigger.CannotReproduce);
            _issueTracker.ApplyTrigger(Trigger.ConfirmNotReproducible);
            Assert.AreEqual(State.Closed, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void NotReproducible_Reject_GoesToReturned()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.AcceptForFix);
            _issueTracker.ApplyTrigger(Trigger.CannotReproduce);
            _issueTracker.ApplyTrigger(Trigger.RejectNotReproducible);
            Assert.AreEqual(State.Returned, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void Fixing_FixApplied_GoesToFixValidation()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.AcceptForFix);
            _issueTracker.ApplyTrigger(Trigger.FixApplied);
            Assert.AreEqual(State.FixValidation, _issueTracker.CurrentState);
            Assert.AreEqual("Tester", _issueTracker.Assignee);
        }

        [TestMethod]
        public void FixValidation_ValidationPassed_GoesToClosed()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.AcceptForFix);
            _issueTracker.ApplyTrigger(Trigger.FixApplied);
            _issueTracker.ApplyTrigger(Trigger.ValidationPassed);
            Assert.AreEqual(State.Closed, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void FixValidation_ValidationFailed_GoesToReturned()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.AcceptForFix);
            _issueTracker.ApplyTrigger(Trigger.FixApplied);
            _issueTracker.ApplyTrigger(Trigger.ValidationFailed);
            Assert.AreEqual(State.Returned, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void Returned_ReturnToAnalysis_GoesToDefectAnalysis()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.MarkAsNotDefect);
            _issueTracker.ApplyTrigger(Trigger.ReturnToAnalysis);
            Assert.AreEqual(State.DefectAnalysis, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void Closed_ReopenDefect_GoesToDefectAnalysis()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.AcceptForFix);
            _issueTracker.ApplyTrigger(Trigger.FixApplied);
            _issueTracker.ApplyTrigger(Trigger.ValidationPassed);
            _issueTracker.ApplyTrigger(Trigger.ReopenDefect);
            Assert.AreEqual(State.DefectAnalysis, _issueTracker.CurrentState);
        }

        [TestMethod]
        public void InvalidTrigger_InitialState_ThrowsException()
        {
            bool exceptionCaught = false;
            try
            {
                _issueTracker.ApplyTrigger(Trigger.AcceptForFix);
            }
            catch (InvalidOperationException)
            {
                exceptionCaught = true;
            }
            Assert.IsTrue(exceptionCaught, "Expected InvalidOperationException to be thrown.");
        }

        [TestMethod]
        public void InvalidTrigger_ValidationPassed_FromAnalysis_ThrowsException()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            bool exceptionCaught = false;
            try
            {
                _issueTracker.ApplyTrigger(Trigger.ValidationPassed);
            }
            catch (InvalidOperationException)
            {
                exceptionCaught = true;
            }
            Assert.IsTrue(exceptionCaught, "Expected InvalidOperationException to be thrown.");
        }

        [TestMethod]
        public void CatchException_ManualTryCatch_InvalidTransition()
        {
            bool exceptionCaught = false;
            try
            {
                _issueTracker.ApplyTrigger(Trigger.FixApplied);
            }
            catch (InvalidOperationException)
            {
                exceptionCaught = true;
            }
            Assert.IsTrue(exceptionCaught, "Expected InvalidOperationException to be thrown.");
        }

        [TestMethod]
        public void IsValidTrigger_ReturnsTrue_ForValidTransitions()
        {
            Assert.IsTrue(_issueTracker.IsValidTrigger(Trigger.StartAnalysis));
        }

        [TestMethod]
        public void IsValidTrigger_ReturnsFalse_ForInvalidTransitions()
        {
            Assert.IsFalse(_issueTracker.IsValidTrigger(Trigger.FixApplied));
        }

        [TestMethod]
        public void GetAllowedTriggers_ReturnsCorrectSet()
        {
            var triggers = _issueTracker.GetAllowedTriggers();
            Assert.IsTrue(triggers.Contains(Trigger.StartAnalysis));
            Assert.AreEqual(1, triggers.Count());
        }

        [TestMethod]
        public void FullHappyPath_Workflow_SuccessfullyCloses()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.AcceptForFix);
            _issueTracker.ApplyTrigger(Trigger.FixApplied);
            _issueTracker.ApplyTrigger(Trigger.ValidationPassed);
            
            Assert.AreEqual(State.Closed, _issueTracker.CurrentState);
        }
        
        [TestMethod]
        public void ReturnedAssignee_IsTester()
        {
            _issueTracker.ApplyTrigger(Trigger.StartAnalysis);
            _issueTracker.ApplyTrigger(Trigger.AcceptForFix);
            _issueTracker.ApplyTrigger(Trigger.FixApplied);
            _issueTracker.ApplyTrigger(Trigger.ValidationFailed);
            
            Assert.AreEqual("Tester", _issueTracker.Assignee);
        }
    }
}
