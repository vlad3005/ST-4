using System;
using Stateless;

namespace BugPro
{
    public enum BugState
    {
        Created,
        Triage,
        Development,
        UnreproducibleCheck,
        CodeReview,
        QA,
        CustomerAcceptance,
        Closed
    }

    public enum BugAction
    {
        BeginTriage,
        
        // Triage to Closed
        MarkAsNotDefect,
        MarkAsDuplicate,
        MarkAsWontFix,
        
        // Triage to Dev
        StartDevelopment,
        
        // Dev to Triage
        PostponeLackOfTime,
        RequireArchitecturalChange,
        MoveToOtherProduct,
        RequestMoreInformation,
        
        // Dev to Unreproducible
        CannotReproduce,
        
        // Unreproducible to Closed/Triage
        ConfirmCannotReproduce,
        RejectCannotReproduce,
        
        // Dev to Review
        FinishDevelopment,
        
        // Review to QA/Dev
        ApproveCodeReview,
        RejectCodeReview,
        
        // QA to CustomerAcceptance/Dev
        PassQA,
        FailQA,
        
        // CustomerAcceptance to Closed/Dev
        AcceptByCustomer,
        RejectByCustomer,
        
        // Closed to Triage
        ReopenIssue
    }

    public class Bug
    {
        private readonly StateMachine<BugState, BugAction> _workflow;

        public BugState CurrentState => _workflow.State;

        public Bug()
        {
            _workflow = new StateMachine<BugState, BugAction>(BugState.Created);
            SetupStateMachine();
        }

        private void SetupStateMachine()
        {
            _workflow.Configure(BugState.Created)
                .Permit(BugAction.BeginTriage, BugState.Triage);

            _workflow.Configure(BugState.Triage)
                .Permit(BugAction.MarkAsNotDefect, BugState.Closed)
                .Permit(BugAction.MarkAsDuplicate, BugState.Closed)
                .Permit(BugAction.MarkAsWontFix, BugState.Closed)
                .Permit(BugAction.StartDevelopment, BugState.Development);

            _workflow.Configure(BugState.Development)
                .Permit(BugAction.PostponeLackOfTime, BugState.Triage)
                .Permit(BugAction.RequireArchitecturalChange, BugState.Triage)
                .Permit(BugAction.MoveToOtherProduct, BugState.Triage)
                .Permit(BugAction.RequestMoreInformation, BugState.Triage)
                .Permit(BugAction.CannotReproduce, BugState.UnreproducibleCheck)
                .Permit(BugAction.FinishDevelopment, BugState.CodeReview);

            _workflow.Configure(BugState.UnreproducibleCheck)
                .Permit(BugAction.ConfirmCannotReproduce, BugState.Closed)
                .Permit(BugAction.RejectCannotReproduce, BugState.Triage);

            _workflow.Configure(BugState.CodeReview)
                .Permit(BugAction.ApproveCodeReview, BugState.QA)
                .Permit(BugAction.RejectCodeReview, BugState.Development);

            _workflow.Configure(BugState.QA)
                .Permit(BugAction.PassQA, BugState.CustomerAcceptance)
                .Permit(BugAction.FailQA, BugState.Development);

            _workflow.Configure(BugState.CustomerAcceptance)
                .Permit(BugAction.AcceptByCustomer, BugState.Closed)
                .Permit(BugAction.RejectByCustomer, BugState.Development);

            _workflow.Configure(BugState.Closed)
                .Permit(BugAction.ReopenIssue, BugState.Triage);
        }

        public void ExecuteAction(BugAction action)
        {
            _workflow.Fire(action);
        }

        public bool CanExecuteAction(BugAction action)
        {
            return _workflow.CanFire(action);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Bug Workflow Tracker ===");
            var bugTracker = new Bug();
            Console.WriteLine($"[1] Initial phase: {bugTracker.CurrentState}");

            bugTracker.ExecuteAction(BugAction.BeginTriage);
            Console.WriteLine($"[2] Action applied [BeginTriage] -> New phase: {bugTracker.CurrentState}");

            bugTracker.ExecuteAction(BugAction.StartDevelopment);
            Console.WriteLine($"[3] Action applied [StartDevelopment] -> New phase: {bugTracker.CurrentState}");

            bugTracker.ExecuteAction(BugAction.CannotReproduce);
            Console.WriteLine($"[4] Action applied [CannotReproduce] -> New phase: {bugTracker.CurrentState}");

            bugTracker.ExecuteAction(BugAction.RejectCannotReproduce);
            Console.WriteLine($"[5] Action applied [RejectCannotReproduce] -> New phase: {bugTracker.CurrentState}");

            bugTracker.ExecuteAction(BugAction.StartDevelopment);
            bugTracker.ExecuteAction(BugAction.FinishDevelopment);
            Console.WriteLine($"[6] Action applied [FinishDevelopment] -> New phase: {bugTracker.CurrentState}");

            bugTracker.ExecuteAction(BugAction.ApproveCodeReview);
            Console.WriteLine($"[7] Action applied [ApproveCodeReview] -> New phase: {bugTracker.CurrentState}");

            bugTracker.ExecuteAction(BugAction.PassQA);
            Console.WriteLine($"[8] Action applied [PassQA] -> New phase: {bugTracker.CurrentState}");

            bugTracker.ExecuteAction(BugAction.AcceptByCustomer);
            Console.WriteLine($"[9] Action applied [AcceptByCustomer] -> New phase: {bugTracker.CurrentState}");
            
            Console.WriteLine("=== Workflow completed successfully ===");
        }
    }
}
