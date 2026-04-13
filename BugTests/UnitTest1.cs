using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;
using System;

namespace BugTests
{
    [TestClass]
    public class BugTests
    {
        private Bug _bug = null!;

        [TestInitialize]
        public void Setup()
        {
            _bug = new Bug();
        }

        [TestMethod]
        public void Test_InitialState_IsNew()
        {
            Assert.AreEqual(State.New, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_CreateBag_TransitionsToAnalysis()
        {
            _bug.Fire(Trigger.Create);
            Assert.AreEqual(State.Analysis, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_AnalysisToFixing_Success()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            Assert.AreEqual(State.Fixing, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_RejectAsNotADefect_Success()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.DeclineNotADefect);
            Assert.AreEqual(State.Closed, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_RejectAsWontFix_Success()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.DeclineWontFix);
            Assert.AreEqual(State.Closed, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_RejectAsDuplicate_Success()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.DeclineDuplicate);
            Assert.AreEqual(State.Closed, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_FixingToAnalysis_NoTimeNow()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.NoTimeNow);
            Assert.AreEqual(State.Analysis, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_FixingToAnalysis_NeedSeparateSolution()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.NeedSeparateSolution);
            Assert.AreEqual(State.Analysis, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_FixingToAnalysis_OtherProductProblem()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.OtherProductProblem);
            Assert.AreEqual(State.Analysis, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_FixingToAnalysis_NeedMoreInfo()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.NeedMoreInfo);
            Assert.AreEqual(State.Analysis, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_FixingToCannotReproduceReview()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.DeveloperCannotReproduce);
            Assert.AreEqual(State.CannotReproduceReview, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_CannotReproduceReview_ToClosed()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.DeveloperCannotReproduce);
            _bug.Fire(Trigger.TesterConfirmCannotReproduce);
            Assert.AreEqual(State.Closed, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_CannotReproduceReview_ToAnalysis()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.DeveloperCannotReproduce);
            _bug.Fire(Trigger.TesterDenyCannotReproduce);
            Assert.AreEqual(State.Analysis, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_FixingToInReview()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.Fixed);
            Assert.AreEqual(State.InReview, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_ReviewPassed_ToTesting()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.Fixed);
            _bug.Fire(Trigger.ReviewPassed);
            Assert.AreEqual(State.Testing, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_ReviewFailed_ToFixing()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.Fixed);
            _bug.Fire(Trigger.ReviewFailed);
            Assert.AreEqual(State.Fixing, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_TestingToDeploying()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.Fixed);
            _bug.Fire(Trigger.ReviewPassed);
            _bug.Fire(Trigger.TesterConfirmFixed);
            Assert.AreEqual(State.Deploying, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_TestingToAnalysis()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.Fixed);
            _bug.Fire(Trigger.ReviewPassed);
            _bug.Fire(Trigger.TesterDenyFixed);
            Assert.AreEqual(State.Analysis, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_DeploySuccess_ToClosed()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.Fixed);
            _bug.Fire(Trigger.ReviewPassed);
            _bug.Fire(Trigger.TesterConfirmFixed);
            _bug.Fire(Trigger.DeploySuccess);
            Assert.AreEqual(State.Closed, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_DeployFailed_ToFixing()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.AssignToFix);
            _bug.Fire(Trigger.Fixed);
            _bug.Fire(Trigger.ReviewPassed);
            _bug.Fire(Trigger.TesterConfirmFixed);
            _bug.Fire(Trigger.DeployFailed);
            Assert.AreEqual(State.Fixing, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_ClosedToAnalysis_Reopen()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.DeclineNotADefect);
            _bug.Fire(Trigger.Reopen);
            Assert.AreEqual(State.Analysis, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_InvalidTransition_CreateWhenAlreadyCreated()
        {
            _bug.Fire(Trigger.Create);
            bool thrown = false;
            try { _bug.Fire(Trigger.Create); } 
            catch (InvalidOperationException) { thrown = true; }
            Assert.IsTrue(thrown, "Expected InvalidOperationException");
        }

        [TestMethod]
        public void Test_InvalidTransition_AssignWhenNew()
        {
            bool thrown = false;
            try { _bug.Fire(Trigger.AssignToFix); } 
            catch (InvalidOperationException) { thrown = true; }
            Assert.IsTrue(thrown, "Expected InvalidOperationException");
        }

        [TestMethod]
        public void Test_InvalidTransition_FixedWhenAnalysis()
        {
            _bug.Fire(Trigger.Create);
            bool thrown = false;
            try { _bug.Fire(Trigger.Fixed); } 
            catch (InvalidOperationException) { thrown = true; }
            Assert.IsTrue(thrown, "Expected InvalidOperationException");
        }

        [TestMethod]
        public void Test_InvalidTransition_CreateWhenClosed()
        {
            _bug.Fire(Trigger.Create);
            _bug.Fire(Trigger.DeclineNotADefect);
            bool thrown = false;
            try { _bug.Fire(Trigger.Create); } 
            catch (InvalidOperationException) { thrown = true; }
            Assert.IsTrue(thrown, "Expected InvalidOperationException");
        }

        [TestMethod]
        public void Test_InvalidTransition_ReviewPassedWhenNotReview()
        {
            _bug.Fire(Trigger.Create);
            bool thrown = false;
            try { _bug.Fire(Trigger.ReviewPassed); } 
            catch (InvalidOperationException) { thrown = true; }
            Assert.IsTrue(thrown, "Expected InvalidOperationException");
        }
    }
}
