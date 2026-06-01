using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;
using System;

namespace BugTests
{
    [TestClass]
    public class WorkflowTests
    {
        private Bug _testBug = null!;

        [TestInitialize]
        public void Init()
        {
            _testBug = new Bug();
        }

        [TestMethod]
        public void Bug_Starts_In_Created_State()
        {
            Assert.AreEqual(BugState.Created, _testBug.CurrentState);
        }

        [TestMethod]
        public void BeginTriage_FromCreated_MovesToTriage()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            Assert.AreEqual(BugState.Triage, _testBug.CurrentState);
        }

        [TestMethod]
        public void Triage_RejectAsNotDefect_ClosesBug()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.MarkAsNotDefect);
            Assert.AreEqual(BugState.Closed, _testBug.CurrentState);
        }

        [TestMethod]
        public void Triage_RejectAsDuplicate_ClosesBug()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.MarkAsDuplicate);
            Assert.AreEqual(BugState.Closed, _testBug.CurrentState);
        }

        [TestMethod]
        public void Triage_RejectAsWontFix_ClosesBug()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.MarkAsWontFix);
            Assert.AreEqual(BugState.Closed, _testBug.CurrentState);
        }

        [TestMethod]
        public void Triage_To_Development_Transition()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            
            Assert.AreEqual(BugState.Development, _testBug.CurrentState);
        }

        [TestMethod]
        public void Development_PostponeLackOfTime_ReturnsToTriage()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.PostponeLackOfTime);
            Assert.AreEqual(BugState.Triage, _testBug.CurrentState);
        }

        [TestMethod]
        public void Development_RequireArchitecturalChange_ReturnsToTriage()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.RequireArchitecturalChange);
            Assert.AreEqual(BugState.Triage, _testBug.CurrentState);
        }

        [TestMethod]
        public void Development_MoveToOtherProduct_ReturnsToTriage()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.MoveToOtherProduct);
            Assert.AreEqual(BugState.Triage, _testBug.CurrentState);
        }

        [TestMethod]
        public void Development_RequestMoreInformation_ReturnsToTriage()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.RequestMoreInformation);
            Assert.AreEqual(BugState.Triage, _testBug.CurrentState);
        }

        [TestMethod]
        public void Developer_CannotReproduce_MovesTo_UnreproducibleCheck()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.CannotReproduce);
            
            Assert.AreEqual(BugState.UnreproducibleCheck, _testBug.CurrentState);
        }

        [TestMethod]
        public void UnreproducibleCheck_Confirm_ClosesBug()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.CannotReproduce);
            _testBug.ExecuteAction(BugAction.ConfirmCannotReproduce);
            
            Assert.AreEqual(BugState.Closed, _testBug.CurrentState);
        }

        [TestMethod]
        public void UnreproducibleCheck_Reject_ReturnsToTriage()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.CannotReproduce);
            _testBug.ExecuteAction(BugAction.RejectCannotReproduce);
            
            Assert.AreEqual(BugState.Triage, _testBug.CurrentState);
        }

        [TestMethod]
        public void FinishDevelopment_MovesTo_CodeReview()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.FinishDevelopment);
            
            Assert.AreEqual(BugState.CodeReview, _testBug.CurrentState);
        }

        [TestMethod]
        public void CodeReview_Reject_ReturnsTo_Development()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.FinishDevelopment);
            _testBug.ExecuteAction(BugAction.RejectCodeReview);
            
            Assert.AreEqual(BugState.Development, _testBug.CurrentState);
        }

        [TestMethod]
        public void CodeReview_Approve_MovesTo_QA()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.FinishDevelopment);
            _testBug.ExecuteAction(BugAction.ApproveCodeReview);
            
            Assert.AreEqual(BugState.QA, _testBug.CurrentState);
        }

        [TestMethod]
        public void QA_Fail_ReturnsTo_Development()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.FinishDevelopment);
            _testBug.ExecuteAction(BugAction.ApproveCodeReview);
            _testBug.ExecuteAction(BugAction.FailQA);
            
            Assert.AreEqual(BugState.Development, _testBug.CurrentState);
        }

        [TestMethod]
        public void QA_Pass_MovesTo_CustomerAcceptance()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.FinishDevelopment);
            _testBug.ExecuteAction(BugAction.ApproveCodeReview);
            _testBug.ExecuteAction(BugAction.PassQA);
            
            Assert.AreEqual(BugState.CustomerAcceptance, _testBug.CurrentState);
        }

        [TestMethod]
        public void Customer_Reject_ReturnsTo_Development()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.FinishDevelopment);
            _testBug.ExecuteAction(BugAction.ApproveCodeReview);
            _testBug.ExecuteAction(BugAction.PassQA);
            _testBug.ExecuteAction(BugAction.RejectByCustomer);
            
            Assert.AreEqual(BugState.Development, _testBug.CurrentState);
        }

        [TestMethod]
        public void Customer_Accept_ClosesBug()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.FinishDevelopment);
            _testBug.ExecuteAction(BugAction.ApproveCodeReview);
            _testBug.ExecuteAction(BugAction.PassQA);
            _testBug.ExecuteAction(BugAction.AcceptByCustomer);
            
            Assert.AreEqual(BugState.Closed, _testBug.CurrentState);
        }

        [TestMethod]
        public void ClosedBug_CanBe_Reopened()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.MarkAsDuplicate);
            _testBug.ExecuteAction(BugAction.ReopenIssue);
            
            Assert.AreEqual(BugState.Triage, _testBug.CurrentState);
        }

        [TestMethod]
        public void InvalidAction_Throws_InvalidOperationException()
        {
            // Bug is in 'Created' state, we try to start development immediately
            bool thrown = false;
            try { _testBug.ExecuteAction(BugAction.StartDevelopment); }
            catch (InvalidOperationException) { thrown = true; }
            Assert.IsTrue(thrown, "Expected InvalidOperationException");
        }

        [TestMethod]
        public void ReopenIssue_ThrowsException_IfBugIsNotClosed()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            // Bug is in Triage, not Closed
            bool thrown = false;
            try { _testBug.ExecuteAction(BugAction.ReopenIssue); }
            catch (InvalidOperationException) { thrown = true; }
            Assert.IsTrue(thrown, "Expected InvalidOperationException");
        }

        [TestMethod]
        public void DuplicateTransition_ThrowsException()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            bool thrown = false;
            try { _testBug.ExecuteAction(BugAction.BeginTriage); }
            catch (InvalidOperationException) { thrown = true; }
            Assert.IsTrue(thrown, "Expected InvalidOperationException");
        }

        [TestMethod]
        public void CanExecuteAction_ReturnsTrue_ForValidAction()
        {
            Assert.IsTrue(_testBug.CanExecuteAction(BugAction.BeginTriage));
        }

        [TestMethod]
        public void CanExecuteAction_ReturnsFalse_ForInvalidAction()
        {
            Assert.IsFalse(_testBug.CanExecuteAction(BugAction.FinishDevelopment));
        }
        
        [TestMethod]
        public void FullWorkflow_SunnyDayScenario()
        {
            _testBug.ExecuteAction(BugAction.BeginTriage);
            _testBug.ExecuteAction(BugAction.StartDevelopment);
            _testBug.ExecuteAction(BugAction.FinishDevelopment);
            _testBug.ExecuteAction(BugAction.ApproveCodeReview);
            _testBug.ExecuteAction(BugAction.PassQA);
            _testBug.ExecuteAction(BugAction.AcceptByCustomer);
            
            Assert.AreEqual(BugState.Closed, _testBug.CurrentState);
        }
    }
}
