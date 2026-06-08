using System;
using Stateless;

namespace BugPro
{
    public enum State
    {
        NewDefect,
        DefectAnalysis,
        FixingInProgress,
        NotReproducible,
        FixValidation,
        Returned,
        Closed,
        Deferred,
        NeedMoreInfo
    }

    public enum Trigger
    {
        StartAnalysis,
        
        MarkAsNotDefect,
        MarkAsDuplicate,
        MarkAsWontFix,
        
        AcceptForFix,
        
        PostponeForLater,
        RequestMoreInfo,
        
        ProvideInfo,
        ResumeAnalysis,
        
        CannotReproduce,
        FixApplied,
        
        ConfirmNotReproducible,
        RejectNotReproducible,
        
        ValidationPassed,
        ValidationFailed,
        
        ReopenDefect,
        ReturnToAnalysis
    }

    public class Bug
    {
        private readonly StateMachine<State, Trigger> _machine;
        public State CurrentState => _machine.State;

        public string? Assignee { get; private set; }

        public Bug()
        {
            _machine = new StateMachine<State, Trigger>(State.NewDefect);
            ConfigureMachine();
        }

        private void ConfigureMachine()
        {
            _machine.Configure(State.NewDefect)
                .Permit(Trigger.StartAnalysis, State.DefectAnalysis);

            _machine.Configure(State.DefectAnalysis)
                .OnEntry(() => Assignee = "Product Team")
                .Permit(Trigger.MarkAsNotDefect, State.Returned)
                .Permit(Trigger.MarkAsDuplicate, State.Returned)
                .Permit(Trigger.MarkAsWontFix, State.Returned)
                .Permit(Trigger.AcceptForFix, State.FixingInProgress)
                .Permit(Trigger.PostponeForLater, State.Deferred)
                .Permit(Trigger.RequestMoreInfo, State.NeedMoreInfo);

            _machine.Configure(State.Deferred)
                .Permit(Trigger.ResumeAnalysis, State.DefectAnalysis);

            _machine.Configure(State.NeedMoreInfo)
                .OnEntry(() => Assignee = "Tester")
                .Permit(Trigger.ProvideInfo, State.DefectAnalysis);

            _machine.Configure(State.FixingInProgress)
                .OnEntry(() => Assignee = "Developer")
                .Permit(Trigger.CannotReproduce, State.NotReproducible)
                .Permit(Trigger.FixApplied, State.FixValidation);

            _machine.Configure(State.NotReproducible)
                .OnEntry(() => Assignee = "Tester")
                .Permit(Trigger.ConfirmNotReproducible, State.Closed)
                .Permit(Trigger.RejectNotReproducible, State.Returned);

            _machine.Configure(State.FixValidation)
                .OnEntry(() => Assignee = "Tester")
                .Permit(Trigger.ValidationPassed, State.Closed)
                .Permit(Trigger.ValidationFailed, State.Returned);

            _machine.Configure(State.Returned)
                .OnEntry(() => Assignee = "Tester")
                .Permit(Trigger.ReturnToAnalysis, State.DefectAnalysis);

            _machine.Configure(State.Closed)
                .Permit(Trigger.ReopenDefect, State.DefectAnalysis);
        }

        public void ApplyTrigger(Trigger trigger)
        {
            _machine.Fire(trigger);
        }

        public bool IsValidTrigger(Trigger trigger)
        {
            return _machine.CanFire(trigger);
        }
        
        public System.Collections.Generic.IEnumerable<Trigger> GetAllowedTriggers()
        {
            return _machine.PermittedTriggers;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Issue Tracking System");
            var issue = new Bug();
            Console.WriteLine($"State: {issue.CurrentState}");

            issue.ApplyTrigger(Trigger.StartAnalysis);
            Console.WriteLine($"Triggered StartAnalysis -> State: {issue.CurrentState}, Assignee: {issue.Assignee}");

            issue.ApplyTrigger(Trigger.AcceptForFix);
            Console.WriteLine($"Triggered AcceptForFix -> State: {issue.CurrentState}, Assignee: {issue.Assignee}");

            issue.ApplyTrigger(Trigger.CannotReproduce);
            Console.WriteLine($"Triggered CannotReproduce -> State: {issue.CurrentState}, Assignee: {issue.Assignee}");

            issue.ApplyTrigger(Trigger.RejectNotReproducible);
            Console.WriteLine($"Triggered RejectNotReproducible -> State: {issue.CurrentState}, Assignee: {issue.Assignee}");

            issue.ApplyTrigger(Trigger.ReturnToAnalysis);
            Console.WriteLine($"Triggered ReturnToAnalysis -> State: {issue.CurrentState}, Assignee: {issue.Assignee}");

            issue.ApplyTrigger(Trigger.AcceptForFix);
            issue.ApplyTrigger(Trigger.FixApplied);
            Console.WriteLine($"Triggered FixApplied -> State: {issue.CurrentState}, Assignee: {issue.Assignee}");

            issue.ApplyTrigger(Trigger.ValidationPassed);
            Console.WriteLine($"Triggered ValidationPassed -> State: {issue.CurrentState}, Assignee: {issue.Assignee}");
            
            Console.WriteLine("End of Demonstration");
        }
    }
}
